using MKW.Core.Notify;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    internal class UserTrustWorker : IDisposable
    {
        private record class Node
        {
            public required ITrustWorkerNode Proxy;
            public required int Depth;
            public required Trust Trust;

            public bool Visited = false;
        }

        private readonly Queue<Node> queue;
        private readonly List<Node> nodes;
        private readonly List<UserInfo> result;

        public UserTrustWorker(UserId start, IEnumerable<ITrustWorkerNode> users)
        {
            queue = new Queue<Node>();
            nodes = [];
            result = [];

            foreach (ITrustWorkerNode user in users)
            {
                if (start == user.Id)
                {
                    Node node = new Node
                    {
                        Proxy = user,
                        Depth = 0,
                        Trust = Trust.SelfTrust,
                    };

                    nodes.Add(node);
                    queue.Enqueue(node);
                }
                else
                {
                    nodes.Add(new Node
                    {
                        Proxy = user,
                        Trust = Trust.Unknown,
                        Depth = -1
                    });
                }
            }
        }

        public bool Iterate()
        {
            if (queue.Count == 0)
            {
                return false;
            }

            Node node = queue.Dequeue();

            VisitNode(node);
            VisitChildren(node);

            return true;
        }

        private void VisitNode(Node node)
        {
            node.Visited = true;

            result.Add(node.Proxy.GetResult(node.Trust));
        }

        private void VisitChildren(Node node)
        {
            foreach (Node childNode in nodes)
            {
                Trust trust = node.Proxy.GetTrust(childNode.Proxy);

                if (trust == Trust.ExplicitTrust && !childNode.Visited)
                {
                    childNode.Depth = node.Depth + 1;
                    childNode.Trust = childNode.Depth switch
                    {
                        0 => Trust.SelfTrust,
                        1 => Trust.ExplicitTrust,
                        _ => Trust.ImplicitTrust,
                    };
                    childNode.Visited = true;

                    queue.Enqueue(childNode);
                }
            }
        }

        public IEnumerable<UserInfo> EnumerateTrustedUsers()
        {
            return result;
        }

        public void Dispose()
        {
        }
    }
}
