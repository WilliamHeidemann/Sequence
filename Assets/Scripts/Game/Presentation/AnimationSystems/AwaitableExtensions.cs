using System;
using UnityEngine;

namespace Game.Presentation.AnimationSystems
{
    public static class AwaitableExtensions
    {
        public static async void Forget(this Awaitable awaitable)
        {
            try
            {
                await awaitable;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        public static async Awaitable<TResult> Then<TSource, TResult>(
            this Awaitable<TSource> task,
            Func<TSource, Awaitable<TResult>> continuation)
        {
            TSource result = await task;
            return await continuation(result);
        }

        public static Func<Awaitable> Apply<T>(this Func<T, Awaitable> func, T input)
        {
            return Method;

            async Awaitable Method()
            {
                await func(input);
            }
        }

        public static Func<Awaitable> Apply<T1, T2>(this Func<T1, T2, Awaitable> func, T1 input1, T2 input2)
        {
            return Method;

            async Awaitable Method()
            {
                await func(input1, input2);
            }
        }
    }
}