using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ValueDisplay
{
    public PresentationType PresentationMode { get; protected set; }

    public abstract float GetFillRate();

    public string GetDisplayText()
    {
        return (PresentationMode switch
        {
            PresentationType.Standard => GetStandardDisplay(),
            PresentationType.Progress => GetProgressDisplay(),
            PresentationType.Percentage => GetPercentageDisplay(),
            _ => string.Empty
        });
    }

    protected abstract string GetStandardDisplay();
    protected abstract string GetProgressDisplay();
    protected abstract string GetPercentageDisplay();
}


public abstract class ValueDisplay<T> : ValueDisplay
{
    public T PrimaryValue { get; protected set; }
    public T SecondaryValue { get; protected set; }

    public ValueDisplay(T newPrimaryValue, T newSecondaryValue, PresentationType presentationMode)
    {
        PrimaryValue = newPrimaryValue;
        SecondaryValue = newSecondaryValue;
    }
}

public class StringDisplay : ValueDisplay<string>
{
    public StringDisplay(string newPrimaryValue, string newSecondaryValue, PresentationType presentationMode) : base(newPrimaryValue, newSecondaryValue, presentationMode) { }

    protected override string GetStandardDisplay() => PrimaryValue + SecondaryValue;
    protected override string GetProgressDisplay() => PrimaryValue + " / " + SecondaryValue;
    protected override string GetPercentageDisplay() => PrimaryValue + "%";
    public override float GetFillRate() => 1f;
}

public class FloatDisplay : ValueDisplay<float>
{
    public FloatDisplay(float newPrimaryValue, float newSecondaryValue, PresentationType presentationMode) : base(newPrimaryValue, newSecondaryValue, presentationMode) { }

    protected override string GetStandardDisplay() => PrimaryValue +  " (" +SecondaryValue + ")";
    protected override string GetProgressDisplay() => PrimaryValue + " / " + SecondaryValue;
    protected override string GetPercentageDisplay() => (Mathf.InverseLerp(0f, PrimaryValue, SecondaryValue) * 100) + "%";
    public override float GetFillRate() => Mathf.InverseLerp(0f, PrimaryValue, SecondaryValue);
}
