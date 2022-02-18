using System.Threading;

namespace MTW.LockFree
{
    /// <summary>
    /// A simple lock-free container - a stack
    /// </summary>
    class LockFreeStack<T>
    {
        public void Push(T value)
        {
            var node = new Node { Value = value };
            do
            {
                node.Next = this._head;
            }
            while (!CompareAndSwap(ref this._head, node.Next, node));
        }

        public T Pop()
        {
            Node node;
            do
            {
                node = this._head;
                if (node == null)
                {
                    return default(T);
                }
            }
            while (!CompareAndSwap(ref this._head, node, node.Next));

            return node.Value;
        }

        class Node
        {
            public T Value;
            public Node Next;
        }

        Node _head;

        static bool CompareAndSwap(ref Node location, Node currentValue, Node newValue)
        {
            return currentValue == Interlocked.CompareExchange(ref location, newValue, currentValue);
        }
    }
}
