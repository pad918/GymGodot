using System.Text;
using System;
using System.Text.Json.Nodes;
using Godot;
using System.Text.Json;
using System.Collections.Generic;

public partial class WebSocketClient : Node
{

    GymGodot Parent => GetParent<GymGodot>();

    WebSocketPeer client = new();

    WebSocketPeer.WriteMode writeMode = WebSocketPeer.WriteMode.Text;

    public void EncodeData(string data)
    {
        //return data.to_utf8_buffer()
    }

    public string DecodeData(byte[] data)
    {
        return Encoding.UTF8.GetString(data);
    }

    public void Init()
    {
        client.Connect("connection_closed", new Callable(this, "_connection_closed"));
        client.Connect("connection_established", new Callable(this, "_connection_established"));
        client.Connect("connection_error", new Callable(this, "_connection_error"));
        client.Connect("data_received", new Callable(this, "_data_received"));
    }


    public void _connection_established(string _protocol)
    {
        //Write mode?
    }
    public void _connection_error()
    {

    }

    public void _connection_closed(string string_was_clean_close)
    {

    }

    public void _peer_connected(string id)
    {

    }



    public override void _Ready()
    {

    }

    public void _data_received() {
        var packet = client.GetPacket();
        var msg = DecodeData(packet);
        var parsedMsg = JsonNode.Parse(msg);

        // Read the received command and call the corresponding public voidtion
        if (parsedMsg["cmd"].ToString() == "reset")
            Parent.Reset();
        else if (parsedMsg["cmd"].ToString() == "step")
            Parent.Step(parsedMsg["action"].Deserialize<List<int>>());
        else if (parsedMsg["cmd"].ToString() == "close")
            Parent.Close();
        else if (parsedMsg["cmd"].ToString() == "render")
            Parent.Render();
        else {
            GD.PrintErr("GYMGODOT : Unrecognized message");
        }
    }

    public void _process(double delta) {
        if (client.GetConnectedHost() == null)
        {
            GD.Print("GYMGODOT : Server connection lost, exit \n");
            GetTree().Quit();
            return;
        }
        client.Poll();
    }

    public void _exit_tree() {
        client.Close();
    }

    public void connect_to_server(string host, int port) {
        var url = $"ws://{host}:{port}";
        client.ConnectToUrl(url);
    }
    public void send_to_server(string data) {
        client.SendText(data);
    }

    public void Close() {
        client.Close();
    }
}