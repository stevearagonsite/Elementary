using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DebugConsole : MonoBehaviour
{
    public delegate void ConsoleCommand();

    public InputField inpField;
    public Text backText;
    public Scrollbar verticalScrollbar;

    private Dictionary<string, ConsoleCommand> _myCommands;
    private Dictionary<string, string> _descriptions;

    private void OnEnable()
    {
        inpField.Select();
        inpField.text = "";
        inpField.ActivateInputField();
    }

    private void OnDisable()
    {
        ClearConsole();
    }

    private void Awake()
    {
        //instancio los diccionarios, similar a como hacemos con arrays o listas
        _myCommands = new Dictionary<string, ConsoleCommand>();
        _descriptions = new Dictionary<string, string>();
    }

    void Start ()
    {
        //agrego los comandos
        AddCommands("!help", ShowHelp, "EL BOTON ROJO");
        AddCommands("cls", ClearConsole, "Clears past actions from log");
        AddCommands("!next", LoadNextLevel, "Load next Level");
        AddCommands("!restart", RestartLevel, "Restart Level");
        AddCommands("!last", LoadPreviousLevel, "Load previous Level");
        AddCommands("!test", LoadTestLevel, "Load Test Level");

        AddCommands("!prototype " + 1, () => LoadPrototype(1), "Load prototype Level " + 1);
        AddCommands("!prototype " + 2, () => LoadPrototype(2), "Load prototype Level " + 2);
        AddCommands("!prototype " + 3, () => LoadPrototype(3), "Load prototype Level " + 3);
        AddCommands("!prototype " + 4, () => LoadPrototype(4), "Load prototype Level " + 4);
        AddCommands("!prototype " + 5, () => LoadPrototype(5), "Load prototype Level " + 5);
        AddCommands("!p " + 0, () => SceneManager.LoadScene("Level-01"), "Load prototype Level " + 0);
        AddCommands("!p " + 1, () => LoadPrototype(1), "Load prototype Level " + 1);
        AddCommands("!p " + 2, () => LoadPrototype(2), "Load prototype Level " + 2);
        AddCommands("!p " + 3, () => LoadPrototype(3), "Load prototype Level " + 3);
        AddCommands("!p " + 4, () => LoadPrototype(4), "Load prototype Level " + 4);
        AddCommands("!p " + 5, () => LoadPrototype(5), "Load prototype Level " + 5);
        //for (int i = 1; i < 4; i++){
        //    AddCommands("!prototype " + i, () => LoadPrototype(i), "Load prototype Level " + i);
        //    AddCommands("!p " + i, () => LoadPrototype(i), "Load prototype Level " + i);
        //}

    }

    private void LoadTestLevel()
    {
        SceneManager.LoadScene("Test-Cris");
    }

    private void LoadPrototype(int value)
    {
        MasterManager.nextScene = value + 1;
        SceneManager.LoadScene("LoadingScreen");
    }

    public void AddCommands(string cheat, ConsoleCommand com, string description)

    {
        _myCommands.Add(cheat, com);
        _descriptions.Add(cheat, description);
    }

    public void RemoveCommand(string cm)
    {
        _myCommands.Remove(cm);
        _descriptions.Remove(cm);
    }

    public void CheckInput()
    {
        //chequeo si el comando existe en el diccionario, si no tiro un mensaje
        if (_myCommands.ContainsKey(inpField.text))
            _myCommands[inpField.text]();
        else
            backText.text += "El comando " + inpField.text + " no existe\n";

        //borro lo escrito por el usuario
        inpField.text = "";
        //pongo el scroll abajo de todo, para que se muestre siempre lo ultimo que aparecio en el log.
        verticalScrollbar.value = 0;
    }

    private void ClearConsole()
    {
        backText.text = "";
    }

    private void ShowHelp()
    {
        string result = "";
        foreach (var elem in _descriptions)
            result += elem.Key + ": " + elem.Value + "\n";

        backText.text += result;
    }

    private void LoadNextLevel()
    {
        LevelManager.instance.NextLevel(null);
    }

    private void LoadPreviousLevel()
    {
        LevelManager.instance.PreviousLevel();
    }

    private void RestartLevel()
    {
        LevelManager.instance.RestartLevel(null);
    }
}
