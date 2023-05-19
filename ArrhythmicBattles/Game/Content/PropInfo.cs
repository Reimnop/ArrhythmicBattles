using ArrhythmicBattles.Util;
using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Game.Content;

public struct PropInfo : IJsonSerializable<PropInfo>
{
    public Identifier Identifier { get; }
    public Vector3 Position { get; }
    public Vector3 Scale { get; }
    public Quaternion Rotation { get; }
    
    public PropInfo(Identifier identifier, Vector3 position, Vector3 scale, Quaternion rotation)
    {
        Identifier = identifier;
        Position = position;
        Scale = scale;
        Rotation = rotation;
    }
    
    public JObject ToJson()
    {
        return new JObject
        {
            ["identifier"] = Identifier.ToString(),
            ["position"] = new JArray
            {
                Position.X, 
                Position.Y, 
                Position.Z
            },
            ["scale"] = new JArray
            {
                Scale.X, 
                Scale.Y, 
                Scale.Z
            },
            ["rotation"] = new JArray
            {
                Rotation.X,
                Rotation.Y,
                Rotation.Z,
                Rotation.W
            }
        };
    }
    
    public static PropInfo FromJson(JObject json)
    {
        return new PropInfo(
            json["identifier"].Value<string>(),
            new Vector3(
                json["position"][0].Value<float>(),
                json["position"][1].Value<float>(),
                json["position"][2].Value<float>()
            ),
            new Vector3(
                json["scale"][0].Value<float>(),
                json["scale"][1].Value<float>(),
                json["scale"][2].Value<float>()
            ),
            new Quaternion(
                json["rotation"][0].Value<float>(),
                json["rotation"][1].Value<float>(),
                json["rotation"][2].Value<float>(),
                json["rotation"][3].Value<float>()
            )
        );
    }
}