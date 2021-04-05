using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Collections.Generic;
namespace LoneX.TchilaVirus
{
    public class CollectiblesManager : MonoBehaviourPunCallbacks
    {
        public static Hashtable PickableTable = new Hashtable();
        public static Dictionary<string,Pickable> pickables = new Dictionary<string, Pickable>();
        
        public new void OnDisable()
        {
            pickables = new Dictionary<string, Pickable>();
            PickableTable = new Hashtable();
        }
        
        public static void AddPickable(Pickable _pickable , bool _state)
        {
            Debug.Log($"{_pickable.Id} added");
            PickableTable.Add(_pickable.Id , _state);
            pickables.Add(_pickable.Id , _pickable);
            PhotonNetwork.CurrentRoom.SetCustomProperties(PickableTable);
        }

        public static void UpdateItemState(Pickable _pickable)
        {
            PickableTable.Remove(_pickable.Id);
            PickableTable.Add(_pickable.Id , _pickable.isPicked);
            PhotonNetwork.CurrentRoom.SetCustomProperties(PickableTable);
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            base.OnRoomPropertiesUpdate(propertiesThatChanged);
            
            foreach(string _propName in propertiesThatChanged.Keys)
            {
                if(_propName.Contains("pickable"))
                {
                    PickableTable.Remove(_propName);
                    PickableTable.Add(_propName , propertiesThatChanged[_propName]);
                }else return;
                
                bool _itemPicked = bool.Parse(propertiesThatChanged[_propName].ToString());
                
                if(pickables.ContainsKey(_propName))
                {
                    pickables[_propName].isPicked = _itemPicked;
                    pickables[_propName].Picked(); 
                } 
            }
        }
    }
}