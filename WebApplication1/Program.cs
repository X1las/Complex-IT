using System;
using Xunit;

string? s1 = null;
string? s2 = s1 ?? "Default Value";
string? s3 = s1 ??= "Default Value";

public static num int operator +(num a, num b)
{
    return a + b;
}