
namespace EJETAGame {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    public interface IInteractable
    {
        void Interact(); //Called when we want to interact with the gameobject, eg. when clicking a button;
        void OnInteractEnter(); //Called when detection with the object starts;
        void OnInteractExit();  //Called when detection with the object ends;
    }

    /// <summary>
    /// Interface for objects that require a long-press interaction (like picking up keys)
    /// </summary>
    public interface ILongPressInteractable : IInteractable
    {
        float RequiredHoldTime { get; } //How long the player needs to hold the key (in seconds)
        void OnLongPressStart(); //Called when the player starts holding the key
        void OnLongPressUpdate(float progress); //Called every frame while holding (progress 0-1)
        void OnLongPressComplete(); //Called when the hold is completed
        void OnLongPressCancel(); //Called if the player releases before completing
    }
}

