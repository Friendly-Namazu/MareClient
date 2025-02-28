﻿namespace MareSynchronos.PlayerData.Data;

public class CharacterDataFragment
{
    public string CustomizePlusScale { get; set; } = string.Empty;
    public HashSet<FileReplacement> FileReplacements { get; set; } = [];
    public string GlamourerString { get; set; } = string.Empty;
}
