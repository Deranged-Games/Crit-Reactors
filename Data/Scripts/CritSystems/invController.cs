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
  public class InvController{
    IMyInventory inv;
    List<IMyInventoryItem> items;

    public InvController(IMyTerminalBlock entity){
      var block = entity as IMyReactor;
      inv = block.GetInventory() as IMyInventory;
      items = inv.GetItems();
    }

    public void outputInventory(){
      for(int i = 0; i < items.Count; i++){
        MyAPIGateway.Utilities.ShowNotification(items[i].Content.SubtypeName, 15000, MyFontEnum.Red);
        MyAPIGateway.Utilities.ShowNotification(items[i].Amount.ToString(), 15000, MyFontEnum.Red);
        MyAPIGateway.Utilities.ShowNotification(items[i].Content.TypeId.ToString(), 15000, MyFontEnum.Red);
      }
    }

    public int getInvAmount(string type, string subType){
      for(int i = 0; i < items.Count; i++){
        if(items[i].Content.TypeId.ToString() == type && items[i].Content.SubtypeName == subType)
          return items[i].Amount.ToIntSafe();
      }
      return 0;
    }
  }
}