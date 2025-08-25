using MKW.Core.Client.Notify;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class UserTrustWorker : IDisposable
    {
        private record class Node
        {
            public required IDatabaseUser DatabaseUser;
            public required int Depth;
            public required Trust Trust;

            public UserTrustProvider? TrustProvider;
            public bool Visited = false;
        }

        private readonly Queue<Node> stack;
        private readonly List<Node> nodes;
        private readonly List<UserInfo> result;
        private readonly ClientSession client;

        public UserTrustWorker(ClientSession client, UserTrustProvider me, IEnumerable<IDatabaseUser> users)
        {
            this.client = client;
            stack = new Queue<Node>();
            nodes = [];
            result = [];

            foreach (IDatabaseUser user in users)
            {
                if (me.UserId == user.Id)
                {
                    Node node = new Node
                    {
                        DatabaseUser = user,
                        Depth = 0,
                        Trust = Trust.SelfTrust,
                        TrustProvider = me,
                    };

                    nodes.Add(node);
                    stack.Enqueue(node);
                }
                else
                {
                    nodes.Add(new Node
                    {
                        DatabaseUser = user,
                        Trust = Trust.Unknown,
                        Depth = -1
                    });
                }
            }
        }

        public bool Iterate()
        {
            if (stack.Count == 0)
            {
                return false;
            }

            Node node = stack.Dequeue();

            VisitNode(node);
            VisitChildren(node);

            return true;
        }

        private void VisitNode(Node node)
        {
            node.Visited = true;

            result.Add(UserInfo.FromDatabaseUser(node.DatabaseUser, node.Trust));
        }

        private Trust GetTrust(Node me, Node node)
        {
            if (me.TrustProvider == null)
            {
                using UserTrustProvider trustProvider = new UserTrustProvider(client, me.DatabaseUser);
                return trustProvider.VerifyTrust2(node.DatabaseUser);
            }
            else
            {
                return me.TrustProvider.VerifyTrust2(node.DatabaseUser);
            }
        }

        private void VisitChildren(Node node)
        {
            foreach (Node childNode in nodes)
            {
                Trust trust = GetTrust(node, childNode);

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

                    stack.Enqueue(childNode);
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
