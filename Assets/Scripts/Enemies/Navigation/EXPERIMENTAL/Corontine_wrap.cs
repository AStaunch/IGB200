using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corontine_wrap
{
    public Coroutine runtime;
    public IEnumerator Target;
    public CoroutineResult Results = new CoroutineResult();
    public CoroutineErrorType ErrorType;
    public bool IsDone { get; private set; } = false;
    private MonoBehaviour Caller_;

    public Corontine_wrap(MonoBehaviour Caller, IEnumerator TargetVoid)
    {
        Caller_ = Caller;
        Target = TargetVoid;
        runtime = Caller_.StartCoroutine(Invoke());
    }

    public IEnumerator Invoke()
    {
        while (Target.MoveNext())
        {
            if (Target.Current == null || Target.Current.GetType() == typeof(WaitForEndOfFrame) || Target.Current.GetType() == typeof(WaitForFixedUpdate))
            {
                Debug.Log($"{Caller_.name} got {Target.Current} from {Target.ToString()}");
                continue;
            }

            Results = new CoroutineResult()
            {
                Result = Target.Current,
                type = Target.Current.GetType()
            };
            
            yield return Results;
        }
        IsDone = true;
    }
}

public class CoroutineResult
{
    public object Result;
    public Type type;
}

public enum CoroutineErrorType
{
    OK,
    FAILED
}
