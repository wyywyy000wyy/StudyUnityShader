using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RB_TREE : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

namespace RB_TREE_IMPL
{

    enum NODE_COLOR
    {
        BLACK,
        RED,
        NONE,
    };

    class NodeDumplicateException : ApplicationException
    {
        public NodeDumplicateException() : base("NodeDumplicateException")
        {
        }
    }

    class Node<K, T> where K : IComparable
    {
        public Node()
        {
            color = NODE_COLOR.NONE;
        }
        public Node(K k, T c){
            this.key = k;
            this.content = c;
            color = NODE_COLOR.NONE;
        }

        public bool isEmpty() { return color == NODE_COLOR.NONE; }

        public bool isLeftChild()
        {
            return parent.left_child.key.CompareTo(key) == 0;
        }

        public Node<K, T> parent;
        public Node<K, T> left_child;
        public Node<K, T> right_child;
        public NODE_COLOR color;
        public K key;
        public T content;

        public Node<K, T> left_most_child()
        {
            Node<K, T> n = left_child;
            while (!n.left_child.isEmpty())
            {
                n = n.left_child;
            }
            return n;
        }

        public Node<K, T> right_most_child()
        {
            Node<K, T> n = right_child;
            while (!n.right_child.isEmpty())
            {
                n = n.right_child;
            }
            return n;
        }
    };

    class Tree<K,T> where K : IComparable
    {
        Tree()
        {
            _root.left_child = root;
            _root.right_child = root;
            root.parent = _root;
        }

        private Node<K, T> _root;
        public Node<K, T> root;
        public void insert(K key, T content)
        {
            Node<K, T> curNode = root;
            if(curNode.isEmpty())
            {
                curNode.key = key;
                curNode.content = content;
                curNode.color = NODE_COLOR.BLACK;
                return;
            }

            Node<K, T> node = new Node<K, T>(key, content);
            node.color = NODE_COLOR.RED;

            if(curNode.key.CompareTo(node.key) > 0)
            {
                curNode.left_child = node;
                node.parent = curNode;
            }
            else if(curNode.key.CompareTo(node.key) < 0)
            {
                curNode.right_child = node;
                node.parent = curNode;
            }
            else
            {
                throw (new DuplicateWaitObjectException());
            }

            check_color(node);
        }

        void check_color(Node<K, T> curNode)
        {
            if(curNode.parent.isEmpty())
            {
                curNode.color = NODE_COLOR.BLACK;
                return;
            }
            Node<K, T> parent = curNode.parent;
            if (parent.color == NODE_COLOR.BLACK)
            {
                return;
            }

            Node<K, T> grandParent = parent.parent;

            if (parent.isLeftChild())
            {
                if (grandParent.right_child.color == NODE_COLOR.RED)
                {
                    grandParent.right_child.color = NODE_COLOR.BLACK;
                    parent.color = NODE_COLOR.BLACK;
                    grandParent.color = NODE_COLOR.RED;
                    check_color(grandParent);
                    return;
                }
                else
                {
                    if(!curNode.isLeftChild())
                    {
                        Node<K, T> leftChild = curNode.left_child;
                        parent.right_child = leftChild;
                        leftChild.parent = parent;

                        curNode.left_child = parent;
                        parent.parent = curNode;

                        curNode.parent = grandParent;
                        grandParent.left_child = curNode;

                        curNode = parent;
                        check_color(curNode);
                        return;
                    }

                    Node<K, T> rightBrother = parent.right_child;
                    grandParent.left_child = rightBrother;
                    rightBrother.parent = grandParent;

                    if(grandParent.isLeftChild())
                    {
                        grandParent.parent.left_child = parent;
                    }
                    else
                    {
                        grandParent.parent.right_child = parent;
                    }
                    parent.parent = grandParent.parent;

                    parent.right_child = grandParent;
                    grandParent.parent = parent;

                    curNode.color = NODE_COLOR.BLACK;
                    curNode = parent;
                    check_color(curNode);
                    return;
                }
            }
            else
            {
                if (grandParent.left_child.color == NODE_COLOR.RED)
                {
                    grandParent.left_child.color = NODE_COLOR.BLACK;
                    parent.color = NODE_COLOR.BLACK;
                    grandParent.color = NODE_COLOR.RED;
                    check_color(grandParent);
                    return;
                }
                else
                {
                    if (!curNode.isLeftChild())
                    {
                        Node<K, T> leftChild = curNode.left_child;
                        parent.right_child = leftChild;
                        leftChild.parent = parent;

                        curNode.left_child = parent;
                        parent.parent = curNode;

                        curNode.parent = grandParent;
                        grandParent.right_child = curNode;

                        curNode = parent;
                        check_color(curNode);
                        return;
                    }

                    grandParent.right_child = curNode;
                    curNode.parent = grandParent;

                    if (grandParent.isLeftChild())
                    {
                        grandParent.parent.left_child = parent;
                    }
                    else
                    {
                        grandParent.parent.right_child = parent;
                    }
                    parent.parent = grandParent.parent;

                    parent.left_child = grandParent;
                    grandParent.parent = parent;

                    curNode = parent;
                    check_color(curNode);
                    return;
                }
            }
        }

        public Node<K, T> delete(K key)
        {
            Node<K, T> toDeleteNode = find(key);
            if(toDeleteNode.isEmpty())
            {
                return toDeleteNode;
            }
            Node<K, T> curNode = toDeleteNode;
            Node<K, T> leftChild = toDeleteNode.left_child;
            if (leftChild.isEmpty())
            {
                toDeleteNode.right_child.parent = toDeleteNode.parent;
                toDeleteNode.right_child.color = toDeleteNode.color;
                if (toDeleteNode.isLeftChild())
                {
                    toDeleteNode.parent.left_child = toDeleteNode.right_child;
                }
                else
                {
                    toDeleteNode.parent.right_child = toDeleteNode.right_child;
                }
                if(toDeleteNode == root)
                {
                    root = toDeleteNode.right_child;
                }
                return toDeleteNode;
            }
            else
            {
                Node<K, T> leftChildRightMost = leftChild.right_most_child();
                if (leftChildRightMost.isEmpty())
                {
                    if (toDeleteNode.isLeftChild())
                    {
                        toDeleteNode.parent.left_child = leftChild;
                    }
                    else
                    {
                        toDeleteNode.parent.right_child = leftChild;
                    }
                    if (toDeleteNode == root)
                    {
                        root = leftChild;
                    }
                    leftChild.parent = toDeleteNode.parent;

                    leftChild.right_child = toDeleteNode.right_child;
                    toDeleteNode.right_child.parent = leftChild;

                    if (!leftChild.left_child.isEmpty() || leftChild.color == NODE_COLOR.RED)
                    {
                        leftChild.left_child.color = leftChild.color;
                        leftChild.color = toDeleteNode.color;
                        
                        return toDeleteNode;
                    }

                    toDeleteNode.right_child.color = NODE_COLOR.RED;
                    curNode = leftChild;
                }
                else
                {
                    bool canRet = false;

                    leftChildRightMost.parent.right_child = leftChildRightMost.left_child;
                    leftChildRightMost.left_child.parent = leftChildRightMost.parent;

                    Node<K, T> leftChildRightMostParent = leftChildRightMost.parent;

                    if (!leftChildRightMost.left_child.isEmpty())
                    {
                        leftChildRightMost.left_child.color = leftChildRightMost.color;
                        canRet = true;
                    }

                    if (toDeleteNode.isLeftChild())
                    {
                        toDeleteNode.parent.left_child = leftChildRightMost;
                    }
                    else
                    {
                        toDeleteNode.parent.right_child = leftChildRightMost;
                    }
                    if (toDeleteNode == root)
                    {
                        root = leftChildRightMost;
                    }
                    leftChildRightMost.parent = toDeleteNode.parent;

                    leftChildRightMost.left_child = toDeleteNode.left_child;
                    toDeleteNode.left_child.parent = leftChildRightMost;

                    leftChildRightMost.right_child = toDeleteNode.right_child;
                    toDeleteNode.right_child.parent = leftChildRightMost;
                    if(canRet || leftChildRightMost.color == NODE_COLOR.RED)
                    {
                        return toDeleteNode;
                    }

                    leftChildRightMostParent.left_child.color = NODE_COLOR.RED;

                    curNode = leftChildRightMostParent;
                }
            }

            if(curNode.color == NODE_COLOR.RED)
            {
                curNode.color = NODE_COLOR.BLACK;
                return toDeleteNode;
            }

            while (true)
            {
                if(curNode.parent.isEmpty())
                {
                    curNode.color = NODE_COLOR.BLACK;
                    return toDeleteNode;
                }
                else //if(curNode.parent.color == NODE_COLOR.RED)
                {
                    Node<K, T> grandParent = curNode.parent.parent;

                    Node<K, T> brother = curNode.isLeftChild() ? curNode.parent.left_child : curNode.parent.right_child;

                    brother.parent = grandParent;
                    if (curNode.parent.isLeftChild())
                    {
                        grandParent.left_child = brother;
                    }
                    else
                    {
                        grandParent.right_child = brother;
                    }

                    if (curNode.isLeftChild())
                    {

                        brother.left_child.parent = curNode.parent;
                        curNode.parent.right_child = brother.left_child;

                        brother.left_child = curNode.parent;
                        curNode.parent.parent = brother;

                    }
                    else
                    {
                        brother.right_child.parent = curNode.parent;
                        curNode.parent.right_child = brother.right_child;

                        brother.right_child = curNode.parent;
                        curNode.parent.parent = brother;
                    }

                    if (curNode.parent.color == NODE_COLOR.RED)
                    {
                        return toDeleteNode;
                    }
                    else if(brother.color == NODE_COLOR.RED)
                    {
                        brother.color = NODE_COLOR.BLACK;
                        return toDeleteNode;
                    }
                    else
                    {
                        curNode = brother;
                    }
                }
            }
        }

        Node<K, T> find_impl(K key)
        {
            Node<K, T> curNode = root;

            while (true)
            {
                if (curNode.key.CompareTo(key) > 0)
                {
                    if (!curNode.left_child.isEmpty())
                    {
                        curNode = curNode.left_child;
                    }
                    else
                    {
                        break;
                    }
                }
                else if (curNode.key.CompareTo(key) < 0)
                {
                    if (!curNode.right_child.isEmpty())
                    {
                        curNode = curNode.right_child;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            return curNode;
        }

        public Node<K, T> find(K key)
        {
            Node<K, T> cur = root;
            while (true)
            {
                int ret = cur.key.CompareTo(key);
                if (ret < 0)
                {
                    if(cur.left_child.isEmpty())
                    {
                        break;
                    }
                    cur = cur.left_child;
                }
                else if(ret > 0)
                {
                    if (cur.right_child.isEmpty())
                    {
                        break;
                    }
                    cur = cur.right_child;
                }
                else
                {
                    return cur;
                }
            }
            return new Node<K, T>();
        }
    }
}


