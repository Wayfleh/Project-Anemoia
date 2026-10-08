using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerStats : Resource
{
    public Dictionary<string, int> attributes_list { get; private set; }
    public Dictionary<string, int> skills_list { get; private set; } // max value 10 for skills and attributes
    const int maxValue = 10;
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
    public int health { get; set; } = 100;
    public int blood { get; set; } = 100;
    public int willpower { get; set; } = 100;

    public int ReadAttribute(Attribute attribute)
    {
        bool sucess = attributes_list.TryGetValue(attribute.ToString(), out int result);
        if (!sucess)
            return -1;

        return result;
    }
    public int ReadSkill(Skill skill)
    {
        bool sucess = skills_list.TryGetValue(skill.ToString(), out int result);
        if (!sucess)
            return -1;

        return result;
    }

    public void IncrementSkill(Skill skill)
    {
        if (skills_list[skill.ToString()] < maxValue)
            skills_list[skill.ToString()]++;
    }
    public void IncrementAttribute(Attribute attribute)
    {
        if (attributes_list[attribute.ToString()] < maxValue)
            attributes_list[attribute.ToString()]++;
    }

    public void SetSkill(Skill skill, int new_skill_value)
    {
        skills_list[skill.ToString()] = Math.Clamp(new_skill_value, 0, 10);
    }

    public void SetAttribute(Attribute attribute, int new_attribute_value)
    {
        attributes_list[attribute.ToString()] = Math.Clamp(new_attribute_value, 1, 10);
    }


    public PlayerStats()
    {
        attributes_list = [];
        skills_list = [];
        Attribute[] attribute_names = Enum.GetValues<Attribute>();
        foreach (Attribute attribute_name in attribute_names)
        {
            attributes_list.Add(attribute_name.ToString(), 1);
        }
        Skill[] skills_names = Enum.GetValues<Skill>();
        foreach (Skill skill_name in skills_names)
        {
            skills_list.Add(skill_name.ToString(), 0);
        }

        // PrintDebug();
    }

    public void PrintDebug()
    {
        GD.Print("---ATTRIBUTES---");
        foreach (KeyValuePair<string, int> _attribute in attributes_list)
        {
            GD.Print($"{_attribute.Key}: {_attribute.Value}");
        }
        GD.Print("---SKILLS---");
        foreach (KeyValuePair<string, int> _skill in skills_list)
        {
            GD.Print($"{_skill.Key}: {_skill.Value}");
        }

        GD.Print("health: " + health);
        GD.Print("blood: " + blood);
        GD.Print("willpower: " + willpower);
    }

}
