using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerStats : Resource
{
    Dictionary<string, int> attributes_list;
    Dictionary<string, int> skills_list; // max value 10 for skills and attributes
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
    private int health = 100;
    private int blood = 100;
    private int willpower = 100;

    public int ReadAttribute(Attribute attribute)
    {
        bool sucess = attributes_list.TryGetValue(attribute.ToString(), out int result);
        if (!sucess)
            return -1;

        return result;
    }
    public int ReadSkill(Skill skill)
    {
        bool sucess = attributes_list.TryGetValue(skill.ToString(), out int result);
        if (!sucess)
            return -1;

        return result;
    }

    public void IncrementSkill(Skill skill)
    {
        skills_list[skill.ToString()]++;
    }
    public void IncrementAttribute(Attribute attribute)
    {
        skills_list[attribute.ToString()]++;
    }

    public void SetSkill(Skill skill, int new_skill_value)
    {
        skills_list[skill.ToString()] = new_skill_value;
    }

    public void SetAttribute(Attribute attribute, int new_attribute_value)
    {
        attributes_list[attribute.ToString()] = new_attribute_value;
    }


    public PlayerStats()
    {
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
        GD.Print(attributes_list);
        GD.Print(skills_list);


        /*attributes_list = new()
        {
            ["strength"] = 1,
            ["dexterity"] = 1,
            ["stamina"] = 1,
            ["awareness"] = 0,
            ["brawl"] = 0,
            ["empathy"] = 0,
            ["intimidation"] = 0,
            ["subterfuge"] = 0,
            ["charisma"] = 1,
            ["manipulation"] = 1,
            ["appearance"] = 0,
            ["firearms"] = 0,
            ["larceny"] = 0,
            ["etiquette"] = 0,
            ["crafts"] = 0,
            ["stealth"] = 0,
            ["perception"] = 1,
            ["intelligence"] = 1,
            ["wits"] = 1,
            ["technology"] = 0,
            ["investigation"] = 0,
            ["academics"] = 0,
            ["occult"] = 0,
            ["politics"] = 0,
            ["humanity"] = 7,
            ["willpower"] = 5,
        };
        GD.Print(player_stats);
        */

        // Skill[] skill_enum_values = Enum.GetValues<Skill>()

        //player_stats.TryGetValue(Stat_List.strength.ToString(), out int stat_value);

        // Stat_List[] stats = Enum.GetValues<Stat_List>();
        // foreach (Stat_List stat in stats)
        // {
        // 	player_stats.TryGetValue(stat.ToString(), out int n);
        // 	GD.Print(stat.ToString() + ": " + n);
        // }
    }

}
