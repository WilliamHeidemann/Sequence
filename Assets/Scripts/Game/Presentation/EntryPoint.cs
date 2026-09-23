using System;
using UnityEngine;

namespace Game.Presentation
{
    public enum StartIn
    {
        MainMenu,
        Game
    }

    public enum Mode
    {
        Local,
        Online
    }

    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private StartIn _startIn;
        [SerializeField] private Mode _mode;
        [SerializeField] private GameObject _mainMenu;
        [SerializeField] private GameStarter _gameStarter;

        private void Awake()
        {
            _mainMenu.SetActive(_startIn == StartIn.MainMenu);
            if (_startIn == StartIn.Game) _gameStarter.StartGame(_mode);
        }
    }
}