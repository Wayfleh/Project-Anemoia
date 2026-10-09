using Godot;
using System;
using Godot.Collections;

[Tool] //So that the constructor is run when resource is initialized
[GlobalClass]
public partial class PlayerStats : Resource
{
    const int maxValue = 10;//for skills and attributes
    public enum Attribute
    {
        strength,
        dexterity,
        stamina,
        charisma,
        manipulation,
        composure,
        intelligence,
        wits,
        resolve
    }
    public enum Skill
    {
        athletics,
        brawl,
        firearms,
        stealth,
        larceny,
        insight,
        intimidation,
        performance,
        persuasion,
        streetwise,
        academics,
        awareness,
        investigation,
        occult,
        technology,
    }
    
    [Export] 
    public Dictionary<Attribute, int> attributes_list { get; private set; }
    [Export] 
    public Dictionary<Skill, int> skills_list { get; private set; }
    public int Health { get; set; } = 100;
    public int Blood { get; set; } = 100;
    public int Willpower { get; set; } = 100;

    public int ReadAttribute(Attribute attribute)
    {
        bool sucess = attributes_list.TryGetValue(attribute, out int result);
        if (!sucess)
            return -1;

        return result;
    }
    public int ReadSkill(Skill skill)
    {
        bool sucess = skills_list.TryGetValue(skill, out int result);
        if (!sucess)
            return -1;

        return result;
    }


    public void SetSkill(Skill skill, int new_skill_value)
    {
        skills_list[skill] = Math.Clamp(new_skill_value, 0, 10);
    }

    public void SetAttribute(Attribute attribute, int new_attribute_value)
    {
        attributes_list[attribute] = Math.Clamp(new_attribute_value, 1, 10);
    }


    public PlayerStats() //populate lists when resource is initialized
    {
        attributes_list = [];
        skills_list = [];
        Attribute[] attribute_names = Enum.GetValues<Attribute>();
        foreach (Attribute attribute_name in attribute_names)
        {
            attributes_list.Add(attribute_name, 1);
        }
        Skill[] skills_names = Enum.GetValues<Skill>();
        foreach (Skill skill_name in skills_names)
        {
            skills_list.Add(skill_name, 0);
        }

        PrintDebug();
    }

    public void PrintDebug()
    {
        GD.Print("---ATTRIBUTES---");
        foreach (var _attribute in attributes_list.Keys)
        {
            var value = attributes_list[_attribute];
            GD.Print($"{_attribute}: {value}");
        }
        GD.Print("---SKILLS---");
        foreach (var _skill in skills_list.Keys)
        {
            var value = skills_list[_skill];
            GD.Print($"{_skill}: {value}");
        }

        GD.Print("-------");
        // GD.Print("Health: " + Health);
        GD.PrintRich("[shake rate=20 level=10][color=aqua]Health [/color][/shake]: " + Health);
        GD.PrintRich("[wave][color=DarkRed]Blood[/color][/wave]: " + Blood);
        GD.PrintRich("[pulse freq=1.0 color=LightBlue ease=-2.0]Willpower[/pulse]: " + Willpower);
    }

}
