using System;
using System.Collections.Generic;

namespace NUnitBench.Utils
{
    public delegate TResult CtorFunc<out TResult>();

    public readonly struct CtorInstance<TResult> : IEquatable<CtorInstance<TResult>>
    {
        public CtorFunc<TResult> Ctor => ctor;
        public TResult Instance => instance;

        private readonly CtorFunc<TResult> ctor;
        private readonly TResult instance;

        public CtorInstance(System.Func<TResult> ctor) : this(new CtorFunc<TResult>(ctor)) { }
        public CtorInstance(CtorFunc<TResult> ctor)
        {
            this.ctor = ctor;
            this.instance = ctor();
        }

        public override bool Equals(object obj)
        {
            return obj is CtorInstance<TResult> inst && Equals(inst);
        }
        public bool Equals(CtorInstance<TResult> other)
        {
            return EqualityComparer<CtorFunc<TResult>>.Default.Equals(ctor, other.ctor) &&
                   EqualityComparer<TResult>.Default.Equals(instance, other.instance);
        }
        public override int GetHashCode()
        {
            int hashCode = 820030361;
            hashCode = hashCode * -1521134295 + EqualityComparer<CtorFunc<TResult>>.Default.GetHashCode(ctor);
            hashCode = hashCode * -1521134295 + EqualityComparer<TResult>.Default.GetHashCode(instance);
            return hashCode;
        }
        public static bool operator ==(CtorInstance<TResult> left, CtorInstance<TResult> right) { return left.Equals(right); }
        public static bool operator !=(CtorInstance<TResult> left, CtorInstance<TResult> right) { return !(left == right); }
    }


}