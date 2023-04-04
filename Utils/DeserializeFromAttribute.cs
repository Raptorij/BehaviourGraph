using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeserializeFromAttribute : Attribute
{
    readonly public string previousTypeFullName;
    public DeserializeFromAttribute(string previousTypeFullName)
    {
        this.previousTypeFullName = previousTypeFullName;
    }
}