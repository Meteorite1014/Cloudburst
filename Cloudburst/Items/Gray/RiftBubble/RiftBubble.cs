﻿using System;
using System.Collections.Generic;
using System.Text;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Cloudburst.Items.Gray.RiftBubble
{
    internal class RiftBubble
    {
        public static ItemDef riftBubbleItem;
        public static BuffDef riftBuff;

        public static GameObject riftBubbleIndicator;
        public static Material riftBubbleMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/CritOnUse/matFullCrit.mat").WaitForCompletion();
        public static Material riftBubbleSecondaryMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/NearbyDamageBonus/matNearbyDamageBonusRangeIndicator.mat").WaitForCompletion();
        public static void Setup()
        {
            riftBubbleItem = ScriptableObject.CreateInstance<ItemDef>();
            riftBubbleItem.tier = ItemTier.Tier1;
            riftBubbleItem.name = "itemslowradius";
            riftBubbleItem.nameToken = "ITEM_SLOWRADIUS_NAME";
            riftBubbleItem.descriptionToken = "ITEM_SLOWRADIUS_DESCRIPTION";
            riftBubbleItem.loreToken = "ITEM_SLOWRADIUS_LORE";
            riftBubbleItem.requiredExpansion = Cloudburst.cloudburstExpansion;

            ContentAddition.AddItemDef(riftBubbleItem);

            riftBuff = ScriptableObject.CreateInstance<BuffDef>();
            riftBuff.canStack = false;
            riftBuff.isDebuff = true;
            riftBuff.iconSprite = Cloudburst.CloudburstAssets.LoadAsset<Sprite>("texBuffSlowRadius");
            riftBuff.buffColor = Color.white;

            ContentAddition.AddBuffDef(riftBuff);

            Modules.Language.Add("ITEM_SLOWRADIUS_NAME", "Rift Bubble");
            Modules.Language.Add("ITEM_SLOWRADIUS_PICKUP", "Reduce incoming knockback and weaken nearby enemies.");
            Modules.Language.Add("ITEM_SLOWRADIUS_DESCRIPTION", "In a radius of 5(+5 per stack), weaken nearby enemies. Reduce incoming knockback by 50%");

            riftBubbleIndicator = Cloudburst.CloudburstAssets.LoadAsset<GameObject>("RiftBubbleIndicator");
            BuffWard buffWard = riftBubbleIndicator.AddComponent<BuffWard>();
            buffWard.rangeIndicator = riftBubbleIndicator.transform.GetChild(0);
            buffWard.buffDef = riftBuff;
            buffWard.buffDuration = 2;
            buffWard.floorWard = false;
            buffWard.expires = false;
            buffWard.invertTeamFilter = true;
            buffWard.shape = BuffWard.BuffWardShape.Sphere;
            riftBubbleIndicator.AddComponent<TeamFilter>();

            riftBubbleMaterial = UnityEngine.Object.Instantiate(riftBubbleMaterial);
            riftBubbleMaterial.SetColor("_TintColor", new Color(1f, 1f, 1f, 1f));
            riftBubbleMaterial.SetTexture("_RemapTex", Cloudburst.CloudburstAssets.LoadAsset<Texture>("texRampRiftBubble2"));
            riftBubbleMaterial.SetFloat("_Boost", 0.1f);
            riftBubbleMaterial.SetFloat("_RimPower", 1);
            riftBubbleMaterial.SetFloat("_RimStrength", 1f);
            riftBubbleMaterial.SetFloat("_IntersectionStrength", 1.2f);
            riftBubbleMaterial.SetFloat("_Cull", 1f);


            VFXAttributes attributes = riftBubbleIndicator.AddComponent<VFXAttributes>();
            attributes.vfxPriority = VFXAttributes.VFXPriority.Always;
            attributes.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
            attributes.secondaryParticleSystem = new ParticleSystem[]
            {
                riftBubbleIndicator.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<ParticleSystem>(),
                riftBubbleIndicator.transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<ParticleSystem>(),
                riftBubbleIndicator.transform.GetChild(0).GetChild(2).GetChild(2).GetComponent<ParticleSystem>()
            };

            riftBubbleIndicator.transform.GetChild(0).GetChild(1).GetComponent<Renderer>().material = riftBubbleMaterial;

            riftBubbleSecondaryMaterial = UnityEngine.Object.Instantiate(riftBubbleSecondaryMaterial);
            riftBubbleSecondaryMaterial.SetColor("_TintColor", new Color(0.02f, 0.01f, 0.3f, 1f));
            riftBubbleIndicator.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material = riftBubbleSecondaryMaterial;


            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }



        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(riftBuff))
            {
                args.attackSpeedMultAdd -= 0.5f;
                args.moveSpeedMultAdd -= 0.33f;
            }
        }

        private static void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {

            self.AddItemBehavior<RiftBehaviour>(self.inventory.GetItemCount(riftBubbleItem));
            orig(self);
        }
    }
}
