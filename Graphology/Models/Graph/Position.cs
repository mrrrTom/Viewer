using Microsoft.Maui.Controls;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Threading.Channels;

namespace Graphology.Models.Graph
{
    internal struct Position<T> where T : struct, IComparable, IConvertible, IMinMaxValue<T>, IEquatable<T>, IComparable<T>
    {
        //ToDo implement equals - for dictionaries for example, to prevent reflection. when valuetype.equals is called
        //■ Equals must be reflexive; that is, x.Equals(x) must return true.
        //■ Equals must be symmetric; that is, x.Equals(y) must return the same value as 
        //y.Equals(x).
        //■ Equals must be transitive; that is, if x.Equals(y) returns true and y.Equals(z) returns 
        //true, then x.Equals(z) must also return true.
        //■ Equals must be consistent.Provided that there are no changes in the two values being compared, Equals should consistently return true or false.
        //■ Have the type implement the System.IEquatable<T> interface’s Equals method This
        //generic interface allows you to define a type-safe Equals method.Usually, you’ll implement
        //the Equals method that takes an Object parameter to internally call the type-safe Equals
        //method.
        //■ Overload the == and != operator methods Usually, you’ll implement these operator methods to internally call the type-safe Equals method.


        //ToDo after overriding equals, u must to override GetHashCode - for using in Dictionaries - просто сплюсуй биты

        public readonly T X;
        public readonly T Y;

        public Position(T x, T y)
        {
            X = x;
            Y = y;
        }
    }
}
