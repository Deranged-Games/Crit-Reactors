using System.Collections.Generic;
using System;
using VRageMath;

using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Definitions;
using Sandbox.Game.ParticleEffects;
using Sandbox.Game;

using VRage;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.Game.ModAPI.Interfaces;
using VRage.ModAPI;
using VRage.ObjectBuilders;

namespace Dondelium.critsystems{
	[MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
	public class CriticalExplode : MySessionComponentBase{
    bool hasinit = false;
    bool isServer = false;
    //List<MyParticleEffect> explosionEffects = new List<MyParticleEffect>();
    
		public override void UpdateBeforeSimulation(){
			if (!hasinit){
				if (MyAPIGateway.Session == null) return;
				Init();
			}
		}
    
		private void Init(){
			hasinit = true;
			isServer = MyAPIGateway.Session.OnlineMode == MyOnlineModeEnum.OFFLINE || MyAPIGateway.Multiplayer.IsServer;
			if(isServer) MyAPIGateway.Session.DamageSystem.RegisterBeforeDamageHandler(0, damageHook);
    }

    private void damageHook(object target, ref MyDamageInformation info){
      if (info.Type==MyDamageType.Grind) return;

			if (target is IMySlimBlock){
				var entity = target as IMySlimBlock;{
					if (entity.FatBlock is MyReactor){
            var tBlock = entity.FatBlock as IMyTerminalBlock;
            var block = entity.FatBlock as IMyReactor;

            float threshold = (1 - (block.CurrentOutput / block.MaxOutput)) * 0.08f + 0.02f;
            if(tBlock.IsWorking && block.Enabled && info.Amount > (threshold * entity.MaxIntegrity)){
              //Set initial dmg via output.
              float dmg = block.CurrentOutput * 500f;

              //Handle inv and get damage from fuel.
              InvController invCon = new InvController(tBlock);
              int fuel = invCon.getInvAmount("MyObjectBuilder_Ingot", "Uranium");
              dmg += 50 * fuel / MyAPIGateway.Session.SessionSettings.BlocksInventorySizeMultiplier;
              float radius = dmg / 2000f;
              if(dmg > 1000000f) dmg = 1000000f;
              if(radius > 200f) radius = 200f;

              //Make it so a reactor does not explode multiple times.
              block.Enabled = false;

              //Explosion Damage!
              Vector3D myPos = block.GetPosition();
              BoundingSphereD sphere = new BoundingSphereD(myPos, radius);
              MyExplosionInfo bomb = new MyExplosionInfo(dmg, dmg, sphere, MyExplosionTypeEnum.BOMB_EXPLOSION, true, true);
              bomb.CreateParticleEffect = false;
              bomb.LifespanMiliseconds = 150 + (int)radius * 45;
              MyExplosions.AddExplosion(ref bomb, true);

              //Explosion Effects!
              MyParticleEffect explosionEffect = null;
              MyParticlesManager.TryCreateParticleEffect(1047, out explosionEffect, false);
              if (explosionEffect != null){
								explosionEffect.WorldMatrix = block.WorldMatrix;
								explosionEffect.UserScale = radius / 8f;
              }

              //MyAPIGateway.Utilities.ShowNotification(block.EntityId+" "+dmg.ToString()+" "+radius.ToString(), 15000, MyFontEnum.Red);
            }
					}
				}
			}
    }

		protected override void UnloadData(){}
  }
}
