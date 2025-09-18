using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

public partial class GymGodot : Node
{
    WebSocketClient Client => GetNode<WebSocketClient>("");

    // Enable / disable this node
    [Export]
    bool enabled = true;

    // Amount of frames simulated per step. 
    // During each of these frames, the current action will be applied. 
    // Once these frames are elapsed, the reward is computed and returned.
    [Export]
    int stepLength = 2;

    // Reference to the Environment node which must implement the methods {
    // get_observation(), get_reward(), reset(), is_done()
    [Export]
    Environment environment; // NODEPATH? TODO FIX TYPE

    // Default url of the server (if not provided through cmdline arguments)
    string serverIP = "127.0.0.1";
    // Default port of the server (if not provided through cmdline arguments)
    int serverPort = 8000;

    // Default path to store render frames (if not provided through cmdline arguments)
    string renderPath = "./render_frames/";
    // Counter for the rendered frames
    int renderFrameCounter = 0;

    // Print debug logs
    [Export]
    bool debugPrint = false;

    List<int> currentAction = [];

    int frameCounter;


    public override void _Ready()
    {

        if (!enabled)
        {
            Client.Free();
            return;
        }
        // This node will never be paused
        //process_mode = 2 
        // Initialy, the environment is paused
        GetTree().Paused = true;

        // Get the server IP/Port from argument
        var arguments = ParseArguments();
        if (arguments.ContainsKey("serverIP"))
            serverIP = arguments["serverIP"];

        if (arguments.ContainsKey("serverPort"))
            serverPort = arguments["serverPort"].ToInt();

        // Get frame render parameters
        if (arguments.ContainsKey("renderPath"))
            renderPath = arguments["renderPath"];

        // Connect to the ws server using those IP/port
        Client.connect_to_server(serverIP, serverPort);
    }

    public Dictionary<string, string> ParseArguments()
    {
        var arguments = new Dictionary<string, string>();

        foreach (var argument in System.Environment.GetCommandLineArgs())
        {
            // Parse valid command-line arguments into a dictionary
            if (argument.Find("=") > -1)
            {
                var key_value = argument.Split("=");
                string key = key_value[0];
                if (key.StartsWith("--"))
                    for (int i = 0; i < 2; i++) key.Remove(0);
                arguments[key] = key_value[1];
            }
        }

        return arguments;
    }


    public override void _PhysicsProcess(double delta)
    {
        if (!enabled)
            return;

        // Simulate stepLength frames with the current action. 
        // Then pause the game and return the observation/reward/isDone to the server
        if (frameCounter >= stepLength)
        {
            GetTree().Paused = true;
            frameCounter = 0;
            ReturnData();
        }
        else
        {
            if (!GetTree().Paused)
            {
                frameCounter += 1;
                environment.ApplyAction(currentAction);
            }
        }
    }

    // Called by WebSocketClient node when it recieve a step msg
    public void Step(List<int> action)
    {

        // Set the action for this new step and run this step
        currentAction = action;

        GetTree().Paused = false;
    }

    // Called by WebSocketClient node when it recieve a close msg
    public void Close()
    {
        Client.Close();
        GetTree().Quit();
    }

    // Return current observation/reward/isDone to the server
    public void ReturnData()
    {
        List<string> obs = []; //environment.get_observation();


        float reward = 0; //environment.get_reward();

        bool done = false; //environment.is_done();

        string jsonData = $"{{\"observation\": {JsonSerializer.Serialize(obs)}, \"reward\": {reward}, \"done\": {done} }}";
        Client.send_to_server(jsonData);
    }

    // Called by WebSocketClient when it recieve a reset msg
    public void Reset()
    {
        environment.Reset();

        List<string> obs = [];  //environment.get_observation()

        string jsonData = $"{{\"init_observation\": {JsonSerializer.Serialize(obs)}}}";

        Client.send_to_server(jsonData);

        renderFrameCounter = 0;
    }

    // Called by WebSocketClient when it recieve a render msg. 
    // Renders to .png in the renderPath folder
    public void Render()
    {
        RenderingServer.ForceDraw();

        Image screenshot = GetViewport().GetTexture().GetImage();

        screenshot.FlipY();

        Error error = screenshot.SavePng($"{renderPath}{renderFrameCounter}.png");

        renderFrameCounter++;

        string answer = $"{{ \"render_error\": {error} }}";
        Client.send_to_server(answer);

    }
}