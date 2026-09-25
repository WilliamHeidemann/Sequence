using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Presentation
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private StartIn _startIn;
        [SerializeField] private Mode _mode;
        [SerializeField] private MainMenu _mainMenu;
        [SerializeField] private GameStarter _gameStarter;

        private async Task Awake()
        {
            try
            {
                if (_mode == Mode.Online) await CloudHandler.Initialize(_mainMenu, _gameStarter);
                _mainMenu.gameObject.SetActive(_startIn == StartIn.MainMenu);
                if (_startIn == StartIn.Game && _mode == Mode.Local)
                {
                    _gameStarter.StartLocalGame();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }
    }
}