using System;
using System.Collections.Generic;

[Serializable]
public class GlossaryEntry
{
    public string term;
    public string definition;
}

[Serializable]
public class GlossaryListWrapper
{
    public List<GlossaryEntry> entries;
}