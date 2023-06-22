using ArrhythmicBattles.Core.Physics;
using ArrhythmicBattles.Core.Resource;
using ArrhythmicBattles.Game.Content.Props;
using ArrhythmicBattles.Util;
using OpenTK.Mathematics;

namespace ArrhythmicBattles.Game.Content;

public class PropContent
{
    public Registry<Prop> Registry { get; }

    public PropContent()
    {
        Registry = new Registry<Prop>(Register);
    }

    private static void Register(RegisterDelegate<Prop> register)
    {
        register(new Identifier("grid"), new GridProp());
    }
}