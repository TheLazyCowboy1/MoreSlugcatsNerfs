using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using RWCustom;
using BepInEx;
using MoreSlugcats;
using Vector2 = UnityEngine.Vector2;
using MoreSlugcatsEnums = MoreSlugcats.MoreSlugcatsEnums;
using System.Text.RegularExpressions;
using System.Reflection;
using MonoMod.RuntimeDetour;
using RainMeadowCompat;

#pragma warning disable CS0618

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace Nerfs;

//dependencies:
//Rain Meadow:
[BepInDependency(MeadowCompatSetup.RAIN_MEADOW_ID, BepInDependency.DependencyFlags.SoftDependency)]

[BepInPlugin(MOD_ID, MOD_NAME, MOD_VERSION)]
public partial class Nerfs : BaseUnityPlugin
{
    public const string MOD_ID = "LazyCowboy.MoreSlugcatsNerfs",
        MOD_NAME = "MoreSlugcats Nerfs",
        MOD_VERSION = "1.1.2";

    /*
     * Ideas (in order of priority):
    */


    public static Nerfs Instance;
    private static NerfsModOptions Options;

    public Nerfs()
    {
        try
        {
            Instance = this;
            Options = new NerfsModOptions(this, Logger);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            throw;
        }
    }
    private void OnEnable()
    {
        On.RainWorld.OnModsInit += RainWorldOnOnModsInit;

        RainMeadowCompat.MeadowCompatSetup.InitializeMeadowCompatibility();
    }

    BindingFlags propFlags = BindingFlags.Instance | BindingFlags.Public;
    BindingFlags propFlags2 = BindingFlags.Public | BindingFlags.Static;
    BindingFlags myMethodFlags = BindingFlags.Static | BindingFlags.Public;

    private bool IsInit;
    private void RainWorldOnOnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            if (IsInit) return;

            //Your hooks go here

            //cleanup hooks
            //On.RainWorldGame.ShutDownProcess += RainWorldGameOnShutDownProcess;
            //On.GameSession.ctor += GameSessionOnctor;

            //Gourmand:
            //empty stomach speed modifier
            On.Player.Update += Gourmand_Empty_Speed_Modifier;
            //crafting nerfs
            //On.MoreSlugcats.GourmandCombos.InitCraftingLibrary += Gourmand_Crafting_Nerfs;
            On.MoreSlugcats.GourmandCombos.CraftingResults_ObjectData += Gourmand_Crafting_Nerfs;
            //slide speed
            //On.Player.UpdateBodyMode += Gourmand_Slide_Modifier;
            On.Player.UpdateAnimation += Gourmand_Slide_Modifier;
            //spear damage modifier
            On.Player.ThrownSpear += Gourmand_Spear_Modifier;
            //exhausion modifier
            On.Player.AerobicIncrease += Gourmand_Exhaustion_Modifier;

            //Artificer:
            //scavenger population modifier
            On.WorldLoader.AddSpawnersFromString += Artificer_Scav_Population_Modifier;
            //extra crafting cost
            On.Player.SpitUpCraftedObject += Artificer_Crafting_Cost;
            On.Player.SwallowObject += Artificer_Swallow_Cost;
            //explosion jump modifier
            On.Player.ClassMechanicsArtificer += Artificer_Jump_Modifier;
            //explosion jump food cost
            On.Player.ClassMechanicsArtificer += Artificer_Jump_Food_Cost;
            //explosion jump requires food
            On.Player.ClassMechanicsArtificer += Artificer_Jump_Requires_Food;
            //extra parry cooldown
            On.Player.ClassMechanicsArtificer += Artificer_Parry_Cost;
            //pyro counter for explosion stun
            On.Player.Stun += Artificer_Stun_Pyro_Counter;
            //scavenger meat modifier
            On.CreatureState.ctor += Artificer_Scavenger_Meat;
            //spear throwing angles
            On.Weapon.Thrown += Artificer_Spear_Throw_Angles;
            //both endings
            On.RainWorldGame.BeatGameMode += Artificer_Echo_Reset;
            On.SaveState.GhostEncounter += Artificer_Karma_Raise_Override;
            On.Player.ctor += Artificer_Karma_Reset_Override;
            On.RainWorldGame.GhostShutDown += Artificer_Echo_Screen_Override;
            //Arena: spear throw speed modifier
            On.Player.ThrownSpear += Arena_Artificer_Spear_Throw_Modifier;

            //Rivulet:
            //speed stats modifier
            //On.SlugcatStats.ctor += Rivulet_Stats_Modifier;
            //slide modifier
            //On.Player.UpdateBodyMode += Rivulet_Slide_Modifier;
            On.Player.UpdateAnimation += Rivulet_Slide_Modifier;
            //jump modifier
            On.Player.Jump += Rivulet_Jump_Modifier;
            On.Player.WallJump += Rivulet_WallJump_Modifier;
            //cycle time after cell decrease??? (make cycles short but with precycles)
            On.RainCycle.GetDesiredCycleLength += Rivulet_Cycle_Length_Modifier;
            On.RainCycle.ctor += Rivulet_Precycle_Modifier;
            try
            {
                Hook rainCycleIntensityHook = new Hook( //creates a hook for the "get method" of RainCycle.preCycleRain_Intensity
                    typeof(RainCycle).GetProperty("preCycleRain_Intensity", propFlags).GetGetMethod(),
                    typeof(Nerfs).GetMethod("RainCycle_preCycleRain_Intensity_get", myMethodFlags)
                );
            } catch (Exception ex) { Logger.LogError(ex); }
            //rot wall grab modifiers
            On.DaddyCorruption.Bulb.Update += Rivulet_Rot_Bulb_Modifier;
            On.DaddyCorruption.EatenCreature.BulbInteraction += Rivulet_Rot_Eat_Modifier;

            //Spearmaster:
            //spear food cost (spears per quarter pip)
            On.Player.GrabUpdate += Spearmaster_Spear_Food_Cost;
            //spear foodless exhaustion
            On.Player.GrabUpdate += Spearmaster_Spear_Exhaustion_Penalty;
            //throw spear exhaustion
            On.Player.ThrownSpear += Spearmaster_Throw_Spear_Exhaustion;
            //bite death chance modifier
            On.Player.DeathByBiteMultiplier += Spearmaster_Bite_Death_Modifier;
            //jump modifier
            On.Player.Jump += Spearmaster_Jump_Modifier;
            On.Player.WallJump += Spearmaster_WallJump_Modifier;
            //spear grow speed modifier
            On.Player.GrabUpdate += Spearmaster_Grow_Speed_Modifier;
            //needle damage modifier
            On.Player.ThrownSpear += Spearmaster_Needle_Damage_Modifier;

            //Saint:
            //tongue jump modifier
            On.Player.TongueUpdate += Saint_Tongue_Modifier;
            //tongue length modifier
            On.Player.Tongue.Shoot += Saint_Shoot_Length_Modifier;
            On.Player.Tongue.ctor += Saint_Max_Tongue_Length_Modifier;
            try
            {
                Hook totalTongueLengthHook = new Hook( //creates a hook for the "get method" of RainCycle.preCycleRain_Intensity
                    typeof(Player.Tongue).GetProperty("totalRope", propFlags).GetGetMethod(),
                    typeof(Nerfs).GetMethod("Player_Tongue_get_totalRope", myMethodFlags)
                );
            }
            catch (Exception ex) { Logger.LogError(ex); }
            //tongue swing modifier
            On.Player.Tongue.Elasticity += Saint_Swing_Modifier;
            //swing length modifier
            On.Player.MovementUpdate += Saint_Lean_Velocity_Modifier;
            //improve tongue aim
            On.Player.Tongue.AutoAim += Saint_Improve_Aim;
            //swallowed warmth modifier
            On.Player.Update += Saint_Swallowed_Warmth_Modifier;
            //lantern warmth modifier
            try
            {
                
                //Hook lanternWarmthHook = new Hook( //creates a hook for the "get method" of Lantern.MoreSlugcats.IProvideWarmth.warmth
                                                   //typeof(Lantern).GetProperty("MoreSlugcats.IProvideWarmth.warmth", propFlags).GetGetMethod(),
                                                   //typeof(IProvideWarmth).GetProperty("warmth", propFlags).GetGetMethod(),
                                                   //typeof(Lantern).GetMethod("MoreSlugcats.IProvideWarmth.get_warmth"),
                    //typeof(Lantern).GetInterface("IProvideWarmth", true).GetProperty("warmth", propFlags).GetGetMethod(),
                    //typeof(Nerfs).GetMethod("IProvideWarmth_get_warmth", myMethodFlags)
                //);
            } catch (Exception ex) { Logger.LogError(ex); }
            //general object warmth
            try
            {
                Hook defaultHeatSourceHook = new Hook( //creates a hook for the "get method" of RainCycle.preCycleRain_Intensity
                    typeof(RainWorldGame).GetProperty("DefaultHeatSourceWarmth", propFlags2).GetGetMethod(),
                    typeof(Nerfs).GetMethod("RainWorldGame_DefaultHeatSourceWarmth_get", myMethodFlags)
                );
            }
            catch (Exception ex) { Logger.LogError(ex); }
            //external object warmth
            On.Creature.HypothermiaUpdate += Saint_Lantern_Modifier;
            //"god timer" modifier
            On.Player.ctor += Saint_God_Timer_Modifier;
            //"god timer" recharge modifier
            On.Player.ClassMechanicsSaint += Saint_Recharge_Modifier;

            //All slugcats:
            On.SlugcatStats.ctor += AllSlugcats_Stats_Modifiers;

            
            MachineConnector.SetRegisteredOI(MOD_ID, Options);
            IsInit = true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            throw;
        }
    }


    #region AllSlugcats
    public void AllSlugcats_Stats_Modifiers(On.SlugcatStats.orig_ctor orig, SlugcatStats self, SlugcatStats.Name slugcat, bool malnourished)
    {
        orig(self, slugcat, malnourished);

        //speed modifiers
        float mod = 1f;
        if (slugcat == MoreSlugcatsEnums.SlugcatStatsName.Gourmand)
            mod = Options.GourmandSpeedModifier.Value;
        else if (slugcat == MoreSlugcatsEnums.SlugcatStatsName.Artificer)
            mod = Options.ArtificerSpeedModifier.Value;
        else if (slugcat == MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
            mod = Options.RivuletSpeedModifier.Value;
        else if (slugcat == MoreSlugcatsEnums.SlugcatStatsName.Spear)
            mod = Options.SpearmasterSpeedModifier.Value;
        else if (slugcat == MoreSlugcatsEnums.SlugcatStatsName.Saint)
            mod = Options.SaintSpeedModifier.Value;

        //if (mod == 1f)
            //return;
        self.runspeedFac *= mod;
        self.corridorClimbSpeedFac *= mod;
        self.poleClimbSpeedFac *= mod;

        //throwing ability modifiers
        if (slugcat == MoreSlugcatsEnums.SlugcatStatsName.Artificer)
            self.throwingSkill = Options.ArtificerThrowingAbility.Value;
        else if (slugcat == MoreSlugcatsEnums.SlugcatStatsName.Spear)
            self.throwingSkill = Options.SpearmasterThrowingAbility.Value;
        else if (slugcat == MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
            self.throwingSkill = Options.RivuletThrowingAbility.Value;
    }
    #endregion

    #region Gourmand
    private Dictionary<EntityID, int> previousFood = new Dictionary<EntityID, int>();
    private int GourmandUpdateCounter = 0;
    public void Gourmand_Empty_Speed_Modifier(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        GourmandUpdateCounter++;
        if (GourmandUpdateCounter < 120)
            return;
        GourmandUpdateCounter = 0;

        if (self.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Gourmand && Options.GourmandEmptySpeedModifier.Value != 1f)
        {
            int food;
            if (previousFood.ContainsKey(self.abstractCreature.ID))
            {
                food = previousFood[self.abstractCreature.ID];
                if (food == self.FoodInStomach)
                    return;
            }
            else
            {
                previousFood.Add(self.abstractCreature.ID, self.FoodInStomach);
                food = self.MaxFoodInStomach;
            }

            //undo previous modifications
            float mod = (float)food / (float)self.MaxFoodInStomach;
            mod = Options.GourmandEmptySpeedModifier.Value * (1 - mod) + mod;
            self.slugcatStats.runspeedFac /= mod;
            self.slugcatStats.corridorClimbSpeedFac /= mod;
            self.slugcatStats.poleClimbSpeedFac /= mod;
            //new modifications
            mod = (float)self.FoodInStomach / (float)self.MaxFoodInStomach;
            mod = Options.GourmandEmptySpeedModifier.Value * (1 - mod) + mod;
            self.slugcatStats.runspeedFac *= mod;
            self.slugcatStats.corridorClimbSpeedFac *= mod;
            self.slugcatStats.poleClimbSpeedFac *= mod;

            previousFood[self.abstractCreature.ID] = self.FoodInStomach;
        }
    }
    public AbstractPhysicalObject.AbstractObjectType Gourmand_Crafting_Nerfs(On.MoreSlugcats.GourmandCombos.orig_CraftingResults_ObjectData orig, Creature.Grasp graspA, Creature.Grasp graspB, bool canMakeMeals)
    {
        AbstractPhysicalObject.AbstractObjectType resultType = orig(graspA, graspB, canMakeMeals);
        if (Options.GourmandCraftingNerfs.Value)
        {
            if (resultType == AbstractPhysicalObject.AbstractObjectType.JellyFish
                || resultType == AbstractPhysicalObject.AbstractObjectType.KarmaFlower
                || resultType == AbstractPhysicalObject.AbstractObjectType.NSHSwarmer
                || resultType == AbstractPhysicalObject.AbstractObjectType.Oracle
                || resultType == AbstractPhysicalObject.AbstractObjectType.OverseerCarcass
                || resultType == AbstractPhysicalObject.AbstractObjectType.SLOracleSwarmer
                || resultType == AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer
                || resultType == AbstractPhysicalObject.AbstractObjectType.VoidSpawn
                || resultType == MoreSlugcatsEnums.AbstractObjectType.Bullet
                || resultType == MoreSlugcatsEnums.AbstractObjectType.EnergyCell
                || resultType == MoreSlugcatsEnums.AbstractObjectType.FireEgg
                || resultType == DLCSharedEnums.AbstractObjectType.SingularityBomb
                || resultType == MoreSlugcatsEnums.AbstractObjectType.MoonCloak
                || resultType == AbstractPhysicalObject.AbstractObjectType.Creature) //ban any live creatures
            {
                resultType = null;
            }
        }

        return resultType;
    }
    public void Gourmand_Slide_Modifier(On.Player.orig_UpdateAnimation orig, Player self)
    {
        Vector2 v1 = self.bodyChunks[0].vel, v2 = self.bodyChunks[1].vel;

        orig(self);

        if (self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Gourmand)
            return;
        if (self.animation != Player.AnimationIndex.BellySlide) //must be sliding
            return;

        float mod = Options.GourmandSlideModifier.Value;
        //float invmod = 1f / mod;
        //float rtmod = Mathf.Sqrt(mod);

        //self.jumpBoost *= mod;
        //new velocity = old velocity + difference * modifier (so the modifier affects total velocity difference now)
        if (self.bodyChunks[0].vel != v1)
        {
            self.bodyChunks[0].vel = v1 + (self.bodyChunks[0].vel - v1) * mod;
            //self.bodyChunks[0].vel.y = v1.y + (self.bodyChunks[0].vel.y - v1.y) * rtmod;
        }
        if (self.bodyChunks[1].vel != v2)
        {
            self.bodyChunks[1].vel = v2 + (self.bodyChunks[1].vel - v2) * mod;
            //self.bodyChunks[1].vel.y = v2.y + (self.bodyChunks[1].vel.y - v2.y) * rtmod;
        }
        /*if (self.mainBodyChunk.vel != v3)
        {
            if (Mathf.Abs(self.mainBodyChunk.vel.x) < Mathf.Abs(v3.x)) //if slowing down
                self.mainBodyChunk.vel = v3 + (self.mainBodyChunk.vel - v3) * invmod;
            else
                self.mainBodyChunk.vel = v3 + (self.mainBodyChunk.vel - v3) * mod;
            //self.mainBodyChunk.vel.y = v3.y + (self.mainBodyChunk.vel.y - v3.y) * rtmod;
        }*/
    }
    public void Gourmand_Spear_Modifier(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
    {
        orig(self, spear);

        if (self.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Gourmand && !self.gourmandExhausted)
        {
            spear.spearDamageBonus *= Options.GourmandSpearDamageModifier.Value;
        }
    }
    public void Gourmand_Exhaustion_Modifier(On.Player.orig_AerobicIncrease orig, Player self, float f)
    {
        float foodpercent = (float)self.FoodInStomach / (float)self.MaxFoodInStomach;
        if (self.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Gourmand)
            orig(self, f * Options.GourmandExhaustionModifier.Value * ((1 - foodpercent) * Options.GourmandEmptyExhaustionModifier.Value + foodpercent));
        else
            orig(self, f);
    }
    #endregion

    #region Artificer
    public void Artificer_Scav_Population_Modifier(On.WorldLoader.orig_AddSpawnersFromString orig, WorldLoader self, string[] line)
    {
        //Don't let it apply to non-story mode games, safari mode games, or non-Artificer campaigns
        if (!self.game.IsStorySession || self.game.rainWorld.safariMode || self.game.StoryCharacter != MoreSlugcatsEnums.SlugcatStatsName.Artificer)
        {
            orig(self, line);
            return;
        }

        //modify line if it contains any useful info about scavs
        bool lineChanged = false;
        string[] array = Regex.Split(line[1], ", ");//Custom.ValidateSpacedDelimiter(line[1], ","), ", ");
        for (int i = 0; i < array.Length; i++)
        {
            string[] array2 = array[i].Split('-');
            CreatureTemplate.Type type = WorldLoader.CreatureTypeFromString(array2[1]);

            bool changeMade = false;
            if (type == CreatureTemplate.Type.Scavenger || type == DLCSharedEnums.CreatureTemplateType.ScavengerElite)
            {
                for (int j = 2; j < array2.Length; j++)
                {
                    if (Int32.TryParse(array2[j], out int scavCount))
                    {
                        changeMade = true;
                        lineChanged = true;
                        array2[j] = Mathf.CeilToInt(scavCount * Options.ArtificerScavPopulationModifier.Value).ToString();
                    }
                }
            }

            if (changeMade)
            {
                string text = "";
                foreach (string s in array2)
                    text += s + "-";
                array[i] = text.Substring(0, text.Length - 1);
            }
        }

        if (lineChanged)
        {
            string text = "";
            foreach (string s in array)
                text += s + ", ";
            line[1] = text.Substring(0, text.Length - 2);
        }

        orig(self, line);
    }
    public void Artificer_Crafting_Cost(On.Player.orig_SpitUpCraftedObject orig, Player self)
    {
        if (self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Artificer || Options.ArtificerCraftingCost.Value <= 1)
        {
            orig(self);
            return;
        }
        int explosivesCount = 0;
        foreach (Creature.Grasp obj in self.grasps)
        {
            if (obj != null && obj.grabbed != null &&
                obj.grabbed.abstractPhysicalObject.type == AbstractPhysicalObject.AbstractObjectType.Spear &&
                (obj.grabbed.abstractPhysicalObject as AbstractSpear).explosive)
                explosivesCount++;
        }

        orig(self);

        int newExplosivesCount = 0;
        foreach (Creature.Grasp obj in self.grasps)
        {
            if (obj != null && obj.grabbed != null &&
                obj.grabbed.abstractPhysicalObject.type == AbstractPhysicalObject.AbstractObjectType.Spear &&
                (obj.grabbed.abstractPhysicalObject as AbstractSpear).explosive)
                newExplosivesCount++;
        }

        if (explosivesCount != newExplosivesCount)
            self.SubtractFood(Options.ArtificerCraftingCost.Value - 1);
    }
    public void Artificer_Swallow_Cost(On.Player.orig_SwallowObject orig, Player self, int grasp)
    {
        if (grasp < 0 || self.grasps[grasp] == null)
        {
            orig(self, grasp);
            return;
        }
        AbstractPhysicalObject obj = self.grasps[grasp].grabbed.abstractPhysicalObject;

        if (self.FoodInStomach < 2 || Options.ArtificerCraftingCost.Value < 2 || self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Artificer)
        {
            orig(self, grasp);
            return;
        }

        orig(self, grasp);

        if (obj.type != self.objectInStomach.type)
            self.SubtractFood(Options.ArtificerCraftingCost.Value - 1);
    }
    public void Artificer_Jump_Modifier(On.Player.orig_ClassMechanicsArtificer orig, Player self)
    {
        Vector2 v1 = self.bodyChunks[0].vel, v2 = self.bodyChunks[1].vel;

        orig(self);

        float mod = Options.ArtificerJumpModifier.Value;
        float rtmod = Mathf.Sqrt(mod);
        //modify jump boost if changed
        if (self.pyroJumpped)
            self.jumpBoost *= rtmod;
        //new velocity = old velocity + difference * modifier (so the modifier affects total velocity difference now)
        if (self.bodyChunks[0].vel != v1)
        {
            self.bodyChunks[0].vel.x = v1.x + (self.bodyChunks[0].vel.x - v1.x) * mod;
            self.bodyChunks[0].vel.y = v1.y + (self.bodyChunks[0].vel.y - v1.y) * rtmod;
        }
        if (self.bodyChunks[1].vel != v2)
        {
            self.bodyChunks[1].vel.x = v2.x + (self.bodyChunks[1].vel.x - v2.x) * mod;
            self.bodyChunks[1].vel.y = v2.y + (self.bodyChunks[1].vel.y - v2.y) * rtmod;
        }
    }
    private int jumpsUntilCost = -1;
    public void Artificer_Jump_Food_Cost(On.Player.orig_ClassMechanicsArtificer orig, Player self)
    {
        float oldParryCooldown = self.pyroParryCooldown;
        float oldJumpCounter = self.pyroJumpCounter;

        orig(self);

        if (Options.ArtificerJumpFoodCost.Value < 1 || Options.ArtificerJumpsPerCost.Value < 1)
            return;

        if (jumpsUntilCost <= 0)
            jumpsUntilCost = Options.ArtificerJumpsPerCost.Value;

        if ((self.pyroJumpCounter > oldJumpCounter || self.pyroParryCooldown > oldParryCooldown))// && Options.ArtificerJumpFoodLoss.Value)
        {
            jumpsUntilCost--;
            if (jumpsUntilCost <= 0)
                Player_SubtractQuarterPips(self, Options.ArtificerJumpFoodCost.Value);
        }
    }
    public void Artificer_Jump_Requires_Food(On.Player.orig_ClassMechanicsArtificer orig, Player self)
    {
        int totalFood = self.playerState.quarterFoodPoints + 4 * self.FoodInStomach;
        if (totalFood >= Options.ArtificerJumpFoodCost.Value || !Options.ArtificerJumpRequiresFood.Value)
        {
            orig(self);
            return;
        }

        int origCanJump = self.canJump;
        self.canJump = 1;
        self.pyroParryCooldown = Mathf.Max(self.pyroParryCooldown, 2f);

        orig(self);

        self.canJump = origCanJump;
    }
    public void Artificer_Parry_Cost(On.Player.orig_ClassMechanicsArtificer orig, Player self)
    {
        float oldParryCooldown = self.pyroParryCooldown;

        orig(self);

        if (Options.ArtificerExtraParryCost.Value <= 0)
            return;

        //if (!self.pyroJumpped && self.pyroParryCooldown > oldParryCooldown)
        if (self.pyroParryCooldown > oldParryCooldown)
        {
            self.pyroJumpCounter += Options.ArtificerExtraParryCost.Value;

            //Copied from decompiled code:
            int num2 = Mathf.Max(1, global::MoreSlugcats.MoreSlugcats.cfgArtificerExplosionCapacity.Value - 3);
            if (self.pyroJumpCounter >= num2)
            {
                self.Stun(60 * (self.pyroJumpCounter - (num2 - 1)));
            }
            if (self.pyroJumpCounter >= global::MoreSlugcats.MoreSlugcats.cfgArtificerExplosionCapacity.Value)
            {
                self.PyroDeath();
            }
        }
    }
    public void Artificer_Stun_Pyro_Counter(On.Player.orig_Stun orig, Player self, int st)
    {
        orig(self, st);

        if (self.stunDamageType == Creature.DamageType.Explosion && self.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Artificer && Options.ArtificerExplosionStunPyroCost.Value > 0)
        {
            self.pyroJumpCounter += Mathf.RoundToInt(st * Options.ArtificerExplosionStunPyroCost.Value / 120f);

            //Copied from decompiled code:
            int num2 = Mathf.Max(1, global::MoreSlugcats.MoreSlugcats.cfgArtificerExplosionCapacity.Value - 3);
            if (self.pyroJumpCounter >= num2)
            {
                //self.Stun(60 * (self.pyroJumpCounter - (num2 - 1)));
            }
            if (self.pyroJumpCounter >= global::MoreSlugcats.MoreSlugcats.cfgArtificerExplosionCapacity.Value)
            {
                self.PyroDeath();
            }
        }
    }
    public void Artificer_Scavenger_Meat(On.CreatureState.orig_ctor orig, CreatureState self, AbstractCreature creature)
    {
        orig(self, creature);

        if (!creature.world.game.IsStorySession || creature.world.game.session.characterStats.name != MoreSlugcatsEnums.SlugcatStatsName.Artificer)
            return;

        if (creature.creatureTemplate.communityID == CreatureCommunities.CommunityID.Scavengers)
            self.meatLeft = Options.ArtificerScavengerMeat.Value;
    }
    public void Artificer_Spear_Throw_Angles(On.Weapon.orig_Thrown orig, Weapon self, Creature thrownBy, UnityEngine.Vector2 thrownPos, UnityEngine.Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
    {
        orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);

        if (throwDir.x == 0) //ignore vertical throws
            return;
        //if (self is not Spear) //ignore non-spear throws
        //return;
        if (Options.ArtificerThrowAngle.Value < 1)
            return;
        if (thrownBy is not Player) //ignore non-player throws
            return;
        Player player = thrownBy as Player;
        if (player.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Artificer && !Options.GeneralArtificerSpearAngles.Value) //ignore non-Artificer throws, unless set to all slugcats
            return;

        //get angle
        Vector2 throwAngle = player.input[0].analogueDir;

        if (Mathf.Abs(throwAngle.x) < 0.05f)
            throwAngle = new Vector2(throwDir.x, player.input[0].y);

        float maxY = throwAngle.x * Mathf.Tan(Mathf.Deg2Rad * Options.ArtificerThrowAngle.Value);
        if (Mathf.Abs(throwAngle.y) > Mathf.Abs(maxY))
            throwAngle.y *= Mathf.Abs(maxY / throwAngle.y);
        throwAngle.y *= Options.ArtificerThrowAngleModifier.Value;
        throwAngle.Normalize();

        self.firstChunk.vel.y = thrownBy.mainBodyChunk.vel.y * 0.5f;
        self.firstChunk.vel.x = thrownBy.mainBodyChunk.vel.x * 0.2f;
        //BodyChunk firstChunk = self.firstChunk;
        //firstChunk.vel.x = firstChunk.vel.x + (float)throwDir.x * 40f * frc;
        self.firstChunk.vel.x += throwAngle.x * 40f * frc;
        //BodyChunk firstChunk2 = self.firstChunk;
        //firstChunk2.vel.y = firstChunk2.vel.y + 1.5f;
        self.firstChunk.vel.y += throwAngle.y * 40f * frc + 0.75f;//1.5f;
    }
    public void Artificer_Echo_Reset(On.RainWorldGame.orig_BeatGameMode orig, RainWorldGame game, bool standardVoidSea)
    {
        //orig(game, standardVoidSea);

        if (!standardVoidSea && game.GetStorySession.saveState.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Artificer && Options.ArtificerBothEndings.Value)
        {
            //foreach (KeyValuePair<GhostWorldPresence.GhostID, int> ghost in game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo)
            //game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[ghost.Key] = 0;
            GhostWorldPresence.GhostID[] keys = game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo.Keys.ToArray();
            foreach (GhostWorldPresence.GhostID key in keys)
                game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[key] = 0;
        }

        orig(game, standardVoidSea);
    }
    public void Artificer_Karma_Raise_Override(On.SaveState.orig_GhostEncounter orig, SaveState self, GhostWorldPresence.GhostID ghost, RainWorld rainWorld)
    {
        if (self.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Artificer && self.deathPersistentSaveData.altEnding && Options.ArtificerBothEndings.Value)
        {
            self.deathPersistentSaveData.altEnding = false;
            orig(self, ghost, rainWorld);
            self.deathPersistentSaveData.altEnding = true;

            //mostly copied from decompiled code, with a few changes
            /*
            Custom.Log(new string[] { string.Format("Save state ghost encounter! {0}", ghost) });
            self.deathPersistentSaveData.ghostsTalkedTo[ghost] = 2;
            int num = 0;
            foreach (KeyValuePair<GhostWorldPresence.GhostID, int> keyValuePair in self.deathPersistentSaveData.ghostsTalkedTo)
            {
                if (keyValuePair.Value > 1)
                {
                    num++;
                }
            }
            int num2 = SlugcatStats.SlugcatStartingKarma(self.saveStateNumber);
            while (num2 < 8 && num > 0) //changed 9 to 8 to ensure Artificer can never properly ascend
            {
                num2++;
                if (num2 == 5)
                {
                    num2++;
                }
                num--;
            }
            if (num2 >= self.deathPersistentSaveData.karmaCap)
            {
                self.deathPersistentSaveData.karmaCap = num2;
            }
            self.deathPersistentSaveData.karma = self.deathPersistentSaveData.karmaCap;
            if (ModManager.MSC)
            {
                self.deathPersistentSaveData.winState.UpdateGhostTracker(self, self.deathPersistentSaveData.winState.GetTracker(MoreSlugcatsEnums.EndgameID.Pilgrim, true) as WinState.BoolArrayTracker);
            }
            */
            rainWorld.progression.SaveProgressionAndDeathPersistentDataOfCurrentState(false, false); //resave
            return;
        }
        else
            orig(self, ghost, rainWorld);
    }
    public void Artificer_Echo_Screen_Override(On.RainWorldGame.orig_GhostShutDown orig, RainWorldGame self, GhostWorldPresence.GhostID ghostID)
    {
        if (self.GetStorySession.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Artificer && self.GetStorySession.saveState.deathPersistentSaveData.altEnding && Options.ArtificerBothEndings.Value)
        {
            self.GetStorySession.saveState.deathPersistentSaveData.altEnding = false;
            orig(self, ghostID);
            self.GetStorySession.saveState.deathPersistentSaveData.altEnding = true;
            //might have to resave the progression data...?
            return;
        }
        else
        orig(self, ghostID);
    }
    public void Artificer_Karma_Reset_Override(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
    {
        //self.GetInitialSlugcatClass();

        //if (self.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Artificer && self.abstractCreature.world.game.IsStorySession
        //    && self.abstractCreature.world.game.GetStorySession.saveState.deathPersistentSaveData.altEnding && Options.ArtificerBothEndings.Value)
        if (world.game.IsStorySession && world.game.GetStorySession.saveState.deathPersistentSaveData.altEnding && Options.ArtificerBothEndings.Value)
        {
            //self.abstractCreature.world.game.GetStorySession.saveState.deathPersistentSaveData.altEnding = false;
            int initKarma = world.game.GetStorySession.saveState.deathPersistentSaveData.karma;
            int initKarmaCap = world.game.GetStorySession.saveState.deathPersistentSaveData.karmaCap;

            orig(self, abstractCreature, world);

            //self.abstractCreature.world.game.GetStorySession.saveState.deathPersistentSaveData.altEnding = true;
            if (self.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Artificer && initKarmaCap != world.game.GetStorySession.saveState.deathPersistentSaveData.karmaCap)
            {
                world.game.GetStorySession.saveState.deathPersistentSaveData.karmaCap = initKarmaCap;
                world.game.GetStorySession.saveState.deathPersistentSaveData.karma = initKarma;
            }

            return;
        }
        else
            orig(self, abstractCreature, world);
    }
    public void Arena_Artificer_Spear_Throw_Modifier(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
    {
        orig(self, spear);

        if ((self.abstractCreature.world.game.IsArenaSession || Options.ArenaArtificerSpearSpeedEverywhere.Value) && self.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Artificer)
            spear.firstChunk.vel *= Options.ArenaArtificerSpearSpeedModifier.Value;
    }
    #endregion

    #region Rivulet
    public void Rivulet_Stats_Modifier(On.SlugcatStats.orig_ctor orig, SlugcatStats self, SlugcatStats.Name slugcat, bool malnourished)
    {
        orig(self, slugcat, malnourished);

        if (slugcat == MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
        {
            self.runspeedFac *= Options.RivuletSpeedModifier.Value;
            self.corridorClimbSpeedFac *= Options.RivuletSpeedModifier.Value;
            self.poleClimbSpeedFac *= Options.RivuletSpeedModifier.Value;
        }
    }
    public void Rivulet_Slide_Modifier(On.Player.orig_UpdateAnimation orig, Player self)
    {
        Vector2 v1 = self.bodyChunks[0].vel, v2 = self.bodyChunks[1].vel;

        orig(self);

        if (self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
            return;
        if (self.animation != Player.AnimationIndex.BellySlide) //must be sliding
            return;

        float mod = Options.RivuletSlideModifier.Value;
        //float invmod = 1f / mod;
        //float rtmod = Mathf.Sqrt(mod);

        //self.jumpBoost *= mod;
        //new velocity = old velocity + difference * modifier (so the modifier affects total velocity difference now)
        if (self.bodyChunks[0].vel != v1)
        {
            self.bodyChunks[0].vel = v1 + (self.bodyChunks[0].vel - v1) * mod;
            //self.bodyChunks[0].vel.y = v1.y + (self.bodyChunks[0].vel.y - v1.y) * rtmod;
        }
        if (self.bodyChunks[1].vel != v2)
        {
            self.bodyChunks[1].vel = v2 + (self.bodyChunks[1].vel - v2) * mod;
            //self.bodyChunks[1].vel.y = v2.y + (self.bodyChunks[1].vel.y - v2.y) * rtmod;
        }
        /*if (self.mainBodyChunk.vel != v3)
        {
            if (Mathf.Abs(self.mainBodyChunk.vel.x) < Mathf.Abs(v3.x)) //if slowing down
                self.mainBodyChunk.vel = v3 + (self.mainBodyChunk.vel - v3) * invmod;
            else
                self.mainBodyChunk.vel = v3 + (self.mainBodyChunk.vel - v3) * mod;
            //self.mainBodyChunk.vel.y = v3.y + (self.mainBodyChunk.vel.y - v3.y) * rtmod;
        }*/
    }
    public void Rivulet_Jump_Modifier(On.Player.orig_Jump orig, Player self)
    {
        Vector2 v1 = self.bodyChunks[0].vel, v2 = self.bodyChunks[1].vel;

        orig(self);

        if (self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
            return;
        float mod = Options.RivuletJumpModifier.Value;
        float rtmod = Mathf.Sqrt(mod); //sqrt used for y because mod is being applied to both self.jumpBoost and vel.y
        self.jumpBoost *= rtmod;
        //new velocity = old velocity + difference * modifier (so the modifier affects total velocity difference now)
        //self.bodyChunks[0].vel = v1 + (self.bodyChunks[0].vel - v1) * mod;
        //self.bodyChunks[1].vel = v2 + (self.bodyChunks[1].vel - v2) * mod;
        self.bodyChunks[0].vel.x = v1.x + (self.bodyChunks[0].vel.x - v1.x) * mod;
        self.bodyChunks[0].vel.y = v1.y + (self.bodyChunks[0].vel.y - v1.y) * rtmod;
        self.bodyChunks[1].vel.x = v2.x + (self.bodyChunks[1].vel.x - v2.x) * mod;
        self.bodyChunks[1].vel.y = v2.y + (self.bodyChunks[1].vel.y - v2.y) * rtmod;

        //y tempering (y jump speed should be < 7f)
        if (Options.RivuletJumpModifier.Value < 1f)
        {
            if (self.bodyChunks[0].vel.y > 0)
                self.bodyChunks[0].vel.y = 7f * Mathf.Sqrt(self.bodyChunks[0].vel.y / 7f);
            if (self.bodyChunks[1].vel.y > 0)
                self.bodyChunks[1].vel.y = 7f * Mathf.Sqrt(self.bodyChunks[1].vel.y / 7f);
        }
    }
    public void Rivulet_WallJump_Modifier(On.Player.orig_WallJump orig, Player self, int direction)
    {
        Vector2 v1 = self.bodyChunks[0].vel, v2 = self.bodyChunks[1].vel;

        orig(self, direction);

        if (self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
            return;
        float mod = Options.RivuletJumpModifier.Value;
        float rtmod = Mathf.Sqrt(mod); //sqrt used for y because mod is being applied to both self.jumpBoost and vel.y
        self.jumpBoost *= rtmod;
        //new velocity = old velocity + difference * modifier (so the modifier affects total velocity difference now)
        //self.bodyChunks[0].vel = v1 + (self.bodyChunks[0].vel - v1) * mod;
        //self.bodyChunks[1].vel = v2 + (self.bodyChunks[1].vel - v2) * mod;
        self.bodyChunks[0].vel.x = v1.x + (self.bodyChunks[0].vel.x - v1.x) * mod;
        self.bodyChunks[0].vel.y = v1.y + (self.bodyChunks[0].vel.y - v1.y) * rtmod;
        self.bodyChunks[1].vel.x = v2.x + (self.bodyChunks[1].vel.x - v2.x) * mod;
        self.bodyChunks[1].vel.y = v2.y + (self.bodyChunks[1].vel.y - v2.y) * rtmod;

        //y tempering (y jump speed should be < 7f)
        if (Options.RivuletJumpModifier.Value < 1f)
        {
            if (self.bodyChunks[0].vel.y > 0)
                self.bodyChunks[0].vel.y = 7f * Mathf.Sqrt(self.bodyChunks[0].vel.y / 7f);
            if (self.bodyChunks[1].vel.y > 0)
                self.bodyChunks[1].vel.y = 7f * Mathf.Sqrt(self.bodyChunks[1].vel.y / 7f);
        }
    }
    public int Rivulet_Cycle_Length_Modifier(On.RainCycle.orig_GetDesiredCycleLength orig, RainCycle self)
    {
        //Cell Extends Cycles
        int cycleLength = 0;
        if (!Options.RivuletCellSlowsCycles.Value && self.world.game.IsStorySession && self.world.game.GetStorySession.saveState.miscWorldSaveData.pebblesEnergyTaken)
        {
            //set taken flag to false, get rain time as if it's false, then restore it to true
            self.world.game.GetStorySession.saveState.miscWorldSaveData.pebblesEnergyTaken = false;
            cycleLength = orig(self);
            self.world.game.GetStorySession.saveState.miscWorldSaveData.pebblesEnergyTaken = true;
        }
        else
            cycleLength = orig(self);

        /*
        //cycle length modifier if precycle
        if (self.maxPreTimer > 0 && self.world.game.GetStorySession.saveState.miscWorldSaveData.pebblesEnergyTaken)
            cycleLength = (int) (cycleLength * Options.RivuletCellCycleModifier.Value);
        */
        return cycleLength;
    }
    public void Rivulet_Precycle_Modifier(On.RainCycle.orig_ctor orig, RainCycle self, World world, float minutes)
    {
        orig(self, world, minutes);

        //precycle if (1. not already a precycle, 2. Rivulet story session, 3. cell taken, 4. chance met)
        if (self.maxPreTimer <= 0 && self.world.game.IsStorySession && 
            self.world.game.StoryCharacter == MoreSlugcatsEnums.SlugcatStatsName.Rivulet && 
            self.world.game.GetStorySession.saveState.miscWorldSaveData.pebblesEnergyTaken
            && (float)UnityEngine.Random.Range(0f, 1f) < Options.RivuletCellPrecycleChance.Value)
        {
            //set up precycle
            self.maxPreTimer = (int)UnityEngine.Random.Range(4800f, 12000f);
            self.preTimer = self.maxPreTimer;
            self.preCycleRainPulse_WaveA = 0f;
            self.preCycleRainPulse_WaveB = 0f;
            //self.preCycleRainPulse_WaveB = 0.3231f;
            self.preCycleRainPulse_WaveC = 1.5707964f;
            //self.preCycleRainPulse_WaveC = 0.7748f;
            world.game.globalRain.preCycleRainPulse_Scale = 1f;

            //recalculate cycle length
            self.cycleLength = (int) (self.cycleLength * Options.RivuletCellCycleModifier.Value);
        }
    }
    public delegate float orig_preCycleRain_Intensity(RainCycle self);
    public static float RainCycle_preCycleRain_Intensity_get(orig_preCycleRain_Intensity orig, RainCycle self)
    {
        if (!self.world.game.IsStorySession ||
            self.world.game.StoryCharacter != MoreSlugcatsEnums.SlugcatStatsName.Rivulet ||
            !self.world.game.GetStorySession.saveState.miscWorldSaveData.pebblesEnergyTaken)
        {
            return orig(self);
        }
        return orig(self) * Options.RivuletCellRainIntensity.Value;
    }
    public void Rivulet_Rot_Bulb_Modifier(On.DaddyCorruption.Bulb.orig_Update orig, DaddyCorruption.Bulb self)
    {
        if (self.eatChunk == null)
        {
            orig(self);
            return;
        }

        if (Options.RivuletRotGrabModifier.Value == 1f || self.eatChunk.owner is not Player || (self.eatChunk.owner as Player).SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
        {
            orig(self);
            return;
        }

        Vector2 initVel = self.eatChunk.vel;
        orig(self);
        self.eatChunk.vel = initVel + Options.RivuletRotGrabModifier.Value * (self.eatChunk.vel - initVel);
    }
    private void Rivulet_Rot_Eat_Modifier(On.DaddyCorruption.EatenCreature.orig_BulbInteraction orig, DaddyCorruption.EatenCreature self, Vector2 newGoalPos, float newGoalPosBulbRad)
    {
        if (Options.RivuletRotGrabModifier.Value == 1f || self.creature is not Player || (self.creature as Player).SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
        {
            orig(self, newGoalPos, newGoalPosBulbRad);
            return;
        }
        float initWait = self.wait;
        orig(self, newGoalPos, newGoalPosBulbRad);
        self.wait = initWait + Options.RivuletRotGrabModifier.Value * (self.wait - initWait);
    }
    #endregion

    #region SpearMaster
    private int spearsUntilCost = -1;
    public void Spearmaster_Spear_Food_Cost(On.Player.orig_GrabUpdate orig, Player self, bool eu)
    {
        if (self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Spear || Options.SpearmasterSpearsUntilCost.Value < 1 || Options.SpearmasterPipsPerSpearCost.Value < 1)
        {
            orig(self, eu);
            return;
        }
        float tailSpecksProgress = -1f;
        try
        {
            tailSpecksProgress = (self.graphicsModule as PlayerGraphics).tailSpecks.spearProg;

            if (spearsUntilCost <= 0)
                spearsUntilCost = Options.SpearmasterSpearsUntilCost.Value;
        } catch (Exception ex) { Logger.LogError(ex); }

        orig(self, eu);

        try
        {
            if (tailSpecksProgress > (self.graphicsModule as PlayerGraphics).tailSpecks.spearProg + 0.5f)
            {
                spearsUntilCost--;
                if (spearsUntilCost <= 0)
                    Player_SubtractQuarterPips(self, Options.SpearmasterPipsPerSpearCost.Value);
            }
        } catch (Exception ex) { Logger.LogError(ex); }
    }
    private void Spearmaster_Spear_Exhaustion_Penalty(On.Player.orig_GrabUpdate orig, Player self, bool eu)
    {
        if (self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Spear || Options.SpearmasterGrowSpearExhaustion.Value <= 0f)
        {
            orig(self, eu);
            return;
        }
        float tailSpecksProgress = -1f;
        try
        {
            tailSpecksProgress = (self.graphicsModule as PlayerGraphics).tailSpecks.spearProg;
        }
        catch (Exception ex) { Logger.LogError(ex); }

        orig(self, eu);

        try
        {
            if (tailSpecksProgress > (self.graphicsModule as PlayerGraphics).tailSpecks.spearProg + 0.5f)
            {
                //foodless
                if (self.playerState.quarterFoodPoints + 4 * self.FoodInStomach <= 0)
                {
                    self.gourmandExhausted = true;
                    self.aerobicLevel = 0.4f + 0.6f * Options.SpearmasterGrowSpearExhaustion.Value;
                }
                else //else, treat like spear throw
                {
                    self.AerobicIncrease(2 * Options.SpearmasterGrowSpearExhaustion.Value); //arbitrary. Spear throw increase = 0.75f, jump = 1f for non-Gourmand
                    if (self.aerobicLevel >= 0.9)
                    {
                        self.gourmandExhausted = true;
                        self.aerobicLevel = 0.4f + 0.6f * Options.SpearmasterThrowSpearExhaustion.Value;
                    }
                }
            }
        }
        catch (Exception ex) { Logger.LogError(ex); }
    }
    public void Spearmaster_Throw_Spear_Exhaustion(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
    {
        orig(self, spear);

        if (self.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Spear && Options.SpearmasterThrowSpearExhaustion.Value > 0f)
        {
            self.AerobicIncrease(2 * Options.SpearmasterThrowSpearExhaustion.Value); //arbitrary. Spear throw increase = 0.75f, jump = 1f for non-Gourmand
            if (self.aerobicLevel >= 0.9)
            {
                self.gourmandExhausted = true;
                self.aerobicLevel = 0.4f + 0.6f * Options.SpearmasterThrowSpearExhaustion.Value;
            }
        }
    }
    public float Spearmaster_Bite_Death_Modifier(On.Player.orig_DeathByBiteMultiplier orig, Player self)
    {
        if (self.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Spear)
            return orig(self) * Options.SpearmasterBiteDeathChanceModifier.Value;
        return orig(self);
    }
    public void Spearmaster_Jump_Modifier(On.Player.orig_Jump orig, Player self)
    {
        Vector2 v1 = self.bodyChunks[0].vel, v2 = self.bodyChunks[1].vel;

        orig(self);

        if (self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Spear)
            return;
        float mod = Options.SpearmasterJumpModifier.Value;
        float rtmod = Mathf.Sqrt(mod); //sqrt used for y because mod is being applied to both self.jumpBoost and vel.y
        self.jumpBoost *= rtmod;
        //new velocity = old velocity + difference * modifier (so the modifier affects total velocity difference now)
        self.bodyChunks[0].vel.x = v1.x + (self.bodyChunks[0].vel.x - v1.x) * mod;
        self.bodyChunks[0].vel.y = v1.y + (self.bodyChunks[0].vel.y - v1.y) * rtmod;
        self.bodyChunks[1].vel.x = v2.x + (self.bodyChunks[1].vel.x - v2.x) * mod;
        self.bodyChunks[1].vel.y = v2.y + (self.bodyChunks[1].vel.y - v2.y) * rtmod;
    }
    public void Spearmaster_WallJump_Modifier(On.Player.orig_WallJump orig, Player self, int direction)
    {
        Vector2 v1 = self.bodyChunks[0].vel, v2 = self.bodyChunks[1].vel;

        orig(self, direction);

        if (self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Spear)
            return;
        float mod = Options.SpearmasterJumpModifier.Value;
        float rtmod = Mathf.Sqrt(mod); //sqrt used for y because mod is being applied to both self.jumpBoost and vel.y
        self.jumpBoost *= rtmod;
        //new velocity = old velocity + difference * modifier (so the modifier affects total velocity difference now)
        //self.bodyChunks[0].vel = v1 + (self.bodyChunks[0].vel - v1) * mod;
        //self.bodyChunks[1].vel = v2 + (self.bodyChunks[1].vel - v2) * mod;
        self.bodyChunks[0].vel.x = v1.x + (self.bodyChunks[0].vel.x - v1.x) * mod;
        self.bodyChunks[0].vel.y = v1.y + (self.bodyChunks[0].vel.y - v1.y) * rtmod;
        self.bodyChunks[1].vel.x = v2.x + (self.bodyChunks[1].vel.x - v2.x) * mod;
        self.bodyChunks[1].vel.y = v2.y + (self.bodyChunks[1].vel.y - v2.y) * rtmod;
    }
    public void Spearmaster_Grow_Speed_Modifier(On.Player.orig_GrabUpdate orig, Player self, bool eu)
    {
        if (self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Spear || Options.SpearmasterGrowSpeedModifier.Value == 1f)
        {
            orig(self, eu);
            return;
        }
        float tailSpecksProgress = -1f;
        try
        {
            tailSpecksProgress = (self.graphicsModule as PlayerGraphics).tailSpecks.spearProg;
        }
        catch (Exception ex) { Logger.LogError(ex); }

        orig(self, eu);

        try
        {
            float difference = (self.graphicsModule as PlayerGraphics).tailSpecks.spearProg - tailSpecksProgress;
            if (difference > 0 && Options.SpearmasterGrowSpeedModifier.Value != 1f)
            {
                (self.graphicsModule as PlayerGraphics).tailSpecks.setSpearProgress(tailSpecksProgress + difference * Options.SpearmasterGrowSpeedModifier.Value);
            }
        }
        catch (Exception ex) { Logger.LogError(ex); }
    }
    public void Spearmaster_Needle_Damage_Modifier(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
    {
        orig(self, spear);

        if (spear.IsNeedle)
            spear.spearDamageBonus *= Options.SpearmasterNeedleDamageModifier.Value;
    }
    #endregion

    #region Saint
    public void Saint_Tongue_Modifier(On.Player.orig_TongueUpdate orig, Player self)
    {
        Vector2 v1 = self.bodyChunks[0].vel, v2 = self.bodyChunks[1].vel;

        orig(self);

        if (self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Saint)
            return;
        if (v1 == self.bodyChunks[0].vel && v2 == self.bodyChunks[1].vel)
            return;

        float mod = Options.SaintTongueModifier.Value;
        float rtmod = Mathf.Sqrt(mod);

        self.jumpBoost *= mod;
        //new velocity = old velocity + difference * modifier (so the modifier affects total velocity difference now)
        if (self.bodyChunks[0].vel != v1)
        {
            self.bodyChunks[0].vel.x = v1.x + (self.bodyChunks[0].vel.x - v1.x) * mod;
            self.bodyChunks[0].vel.y = v1.y + (self.bodyChunks[0].vel.y - v1.y) * rtmod;
        }
        if (self.bodyChunks[1].vel != v2)
        {
            self.bodyChunks[1].vel.x = v2.x + (self.bodyChunks[1].vel.x - v2.x) * mod;
            self.bodyChunks[1].vel.y = v2.y + (self.bodyChunks[1].vel.y - v2.y) * rtmod;
        }
    }
    public void Saint_Shoot_Length_Modifier(On.Player.Tongue.orig_Shoot orig, Player.Tongue self, Vector2 dir)
    {
        orig(self, dir);

        self.requestedRopeLength *= Options.SaintTongueLengthModifier.Value;
    }
    public void Saint_Max_Tongue_Length_Modifier(On.Player.Tongue.orig_ctor orig, Player.Tongue self, Player player, int tongueNum)
    {
        orig(self, player, tongueNum);

        float mod = Options.SaintTongueLengthModifier.Value;
        if (mod != 1f)
        {
            self.idealRopeLength *= mod;
            self.baseIdealRopeLength *= mod;
            self.minRopeLength *= mod;
            self.maxRopeLength *= mod;
        }
    }
    public delegate float orig_Tongue_get_totalRope(Player.Tongue self);
    public static float Player_Tongue_get_totalRope(orig_Tongue_get_totalRope orig, Player.Tongue self)
    {
        return orig(self) * Options.SaintTongueLengthModifier.Value;
    }
    public void Saint_Swing_Modifier(On.Player.Tongue.orig_Elasticity orig, Player.Tongue self)
    {
        Vector2 origVel = self.baseChunk.vel;

        orig(self);

        float xDiff = self.baseChunk.vel.x - origVel.x;
        if (xDiff != 0f && Options.SaintTongueSwingModifier.Value != 1f && false) //temporarily disabled
        {
            //float diffMod = 1f / (1 + (1 / Options.SaintTongueSwingModifier.Value) * Mathf.Max(0, Mathf.Abs((self.baseChunk.pos.x - self.pos.x) / (1f * Options.SaintTongueSwingModifier.Value)) - 1)); //80f is arbitary, presumably 4 tiles
            //diffMod = 1 + (1 - diffMod) * (1 - Mathf.Sqrt(self.elastic));
            float diffMod = Mathf.Pow(1 + Mathf.Abs(self.baseChunk.pos.x - self.pos.x) / (1f * Options.SaintTongueSwingModifier.Value), 3f);
            if (Mathf.Sign(self.baseChunk.vel.x) != Mathf.Sign(self.baseChunk.pos.x - self.pos.x))
                diffMod = 1f / diffMod;
            diffMod = 1 + (diffMod - 1) * (1 - Mathf.Sqrt(self.elastic));
            self.baseChunk.vel.x = origVel.x + xDiff * diffMod;
        }
        self.baseChunk.vel = origVel + Options.SaintSwingSpeedModifier.Value * (self.baseChunk.vel - origVel);
    }
    public void Saint_Lean_Velocity_Modifier(On.Player.orig_MovementUpdate orig, Player self, bool eu)
    {
        float origVelx0 = self.bodyChunks[0].vel.x;
        float origVelx1 = self.bodyChunks[1].vel.x;

        orig(self, eu);

        if (Options.SaintTongueSwingModifier.Value != 1f && self.canJump <= 0 && self.tongue != null && self.tongue.Attached && self.input[0].x != 0)
        {
            float mod = Options.SaintTongueSwingModifier.Value;
            mod = (mod > 1) ? mod : mod * mod;
            //meets basic requirements. is input in correct direction?
            if (self.tongue.baseChunk.pos.x > self.tongue.pos.x == self.input[0].x > 0) {
                self.bodyChunks[0].vel.x = origVelx0 + (self.bodyChunks[0].vel.x - origVelx0) * Options.SaintTongueSwingModifier.Value;
                self.bodyChunks[1].vel.x = origVelx1 + (self.bodyChunks[1].vel.x - origVelx1) * Options.SaintTongueSwingModifier.Value;
            }
        }
    }
    public Vector2 Saint_Improve_Aim(On.Player.Tongue.orig_AutoAim orig, Player.Tongue self, Vector2 originalDir)
    {
        if (!Options.SaintImproveTongueAim.Value)
            return orig(self, originalDir);

        Vector2 dir = self.player.input[0].analogueDir;
        if (Mathf.Abs(dir.x) < 0.01f)
            dir = originalDir;
            //dir = new Vector2(self.player.input[0].x, self.player.input[0].y);
        if (dir.y < 0)
            dir.y = 0;
        dir.Normalize();

        float num = 230f * Options.SaintTongueLengthModifier.Value; //added tongue length modifier

        if (!SharedPhysics.RayTraceTilesForTerrain(self.player.room, self.baseChunk.pos, self.baseChunk.pos + dir * num))
        {
            return dir;
        }
        float num2 = Custom.VecToDeg(dir);
        for (float num3 = 5f; num3 < 30f; num3 += 5f)
        {
            for (float num4 = -1f; num4 <= 1f; num4 += 2f)
            {
                if (!SharedPhysics.RayTraceTilesForTerrain(self.player.room, self.baseChunk.pos, self.baseChunk.pos + Custom.DegToVec(num2 + num3 * num4) * num))
                {
                    return Custom.DegToVec(num2 + num3 * num4);
                }
            }
        }
        return dir;
    }
    public void Saint_Swallowed_Warmth_Modifier(On.Player.orig_Update orig, Player self, bool eu)
    {
        //note: applies to all slugcats, not just Saint. This means that slugcats in Saint's campaign will be affected too

        //mostly copied from decompiled code
        if (self.room != null && self.room.blizzard && Options.SaintSwallowedWarmthModifier.Value != 1f)
        {
            if (self.objectInStomach != null && self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.Lantern)
            {
                //self.Hypothermia -= Mathf.Lerp(RainWorldGame.DefaultHeatSourceWarmth, 0f, self.HypothermiaExposure);
                self.Hypothermia -= (Options.SaintSwallowedWarmthModifier.Value - 1f) * Mathf.Lerp(RainWorldGame.DefaultHeatSourceWarmth, 0f, self.HypothermiaExposure);
            }
        }

        orig(self, eu);
    }
    public delegate float orig_IProvideWarmth_get_warmth(IProvideWarmth self);
    public static float IProvideWarmth_get_warmth(orig_IProvideWarmth_get_warmth orig, IProvideWarmth self)
    {
        if (self.range == 350f) //range of lantern and "lantern stick"
            return orig(self) * Options.SaintLanternWarmthModifier.Value;
        return orig(self);
    }
    public void Saint_God_Timer_Modifier(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
    {
        orig(self, abstractCreature, world);

        if (self.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Saint)
        {
            self.maxGodTime *= Options.SaintGodTimeModifier.Value;
            self.godTimer = self.maxGodTime;
        }
    }
    public void Saint_Recharge_Modifier(On.Player.orig_ClassMechanicsSaint orig, Player self)
    {
        float initGodTimer = self.godTimer;

        orig(self);

        float difference = self.godTimer - initGodTimer;
        if (difference > 0 && Options.SaintRechargeRateModifier.Value != 1f)
        {
            self.godTimer = Mathf.Min(initGodTimer + difference * Options.SaintRechargeRateModifier.Value, self.maxGodTime);
        }
    }
    public delegate float orig_RainWorldGame_DefaultHeatSourceWarmth();
    public static float RainWorldGame_DefaultHeatSourceWarmth_get(orig_RainWorldGame_DefaultHeatSourceWarmth orig)
    {
        return orig() * Options.SaintObjectWarmthModifier.Value;
    }
    public void Saint_Lantern_Modifier(On.Creature.orig_HypothermiaUpdate orig, Creature self)
    {
        //copied from decompiled code, with a few modifications
        if (self.abstractCreature.creatureTemplate.type == CreatureTemplate.Type.Overseer)
        {
            return;
        }
        if (ModManager.MSC && self.room.blizzardGraphics != null && self.room.roomSettings.DangerType == DLCSharedEnums.RoomRainDangerType.Blizzard && self.room.world.rainCycle.CycleProgression > 0f)
        {
            foreach (IProvideWarmth provideWarmth in self.room.blizzardHeatSources)
            {
                float num = Vector2.Distance(self.firstChunk.pos, provideWarmth.Position());
                if (self.abstractCreature.Hypothermia > 0.001f && provideWarmth.loadedRoom == self.room && num < provideWarmth.range)
                {
                    float num2 = Mathf.InverseLerp(provideWarmth.range, provideWarmth.range * 0.2f, num);
                    //self.abstractCreature.Hypothermia -= Mathf.Lerp(provideWarmth.warmth * num2, 0f, self.HypothermiaExposure);
                    self.abstractCreature.Hypothermia -= (Options.SaintLanternWarmthModifier.Value - 1f) * Mathf.Lerp(provideWarmth.warmth * num2, 0f, self.HypothermiaExposure);
                }
            }
        }

        orig(self);
    }
    #endregion

    #region HelperMethods
    //copied from Player.AddQuarterPip
    public void Player_SubtractQuarterPips(Player player, int amount, bool invokeRPC = true)
    {
        if (player.FoodInStomach < 1 && player.playerState.quarterFoodPoints < 1)
        {
            return;
        }

        player.playerState.quarterFoodPoints -= amount;
        if (ModManager.CoopAvailable && player.abstractCreature.world.game.IsStorySession && player.abstractCreature.world.game.Players[0] != player.abstractCreature && !player.isNPC)
        {
            PlayerState playerState = player.abstractCreature.world.game.Players[0].state as PlayerState;
            //JollyCustom.Log(string.Format("Player add quarter food. Amount to add {0}", this.playerState.playerNumber), false);
            playerState.quarterFoodPoints -= amount;
        }
        if (player.playerState.quarterFoodPoints < 0)
        {
            player.playerState.quarterFoodPoints += 4;
            player.SubtractFood(1);
            amount -= 4;
        }

        try
        {
            if (player.room != null)
            {
                foreach (var camera in player.room.game.cameras)
                {
                    if (camera != null && camera.hud.foodMeter.quarterPipShower != null)
                    {
                        camera.hud.foodMeter.quarterPipShower.Reset();
                    }
                }
            }
        } catch (Exception ex) { Logger.LogError(ex); }

        //invoke RPC
        if (invokeRPC)
            SafeMeadowInterface.InvokeSubtractQuarterPipRPC(player, amount);
    }

    public static void LogSomething(object obj)
    {
        Instance.Logger.LogInfo(obj);
    }
    #endregion
}
