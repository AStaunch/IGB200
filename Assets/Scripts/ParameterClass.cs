using System;

public static class ParameterClass
{
    private string name;
    Color[] palette;
    Action<> action;

    public ParameterClass(string Name, Color[] Palette, Action<> Action)
	{
        name = Name;
        palette = Palette;
        action = Action;
	}

    public Behave(GameObject other, float strength)
    {
        action(strength);
    }
}
