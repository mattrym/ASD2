using System.Collections.Generic;
using System.Text;
using ASD;
using System;

namespace Teksty
{

public partial class Huffman
    {

    private class Node
        {

        public char character;
        public long freq;
        public Node left, right;

        public Node(char character, long freq)
            {
            this.character = character;
            this.freq = freq;
            }

        public Node(long freq, Node left, Node right)
            {
            this.freq = freq;
            this.left = left;
            this.right = right;
            }

        }

    private Node root;
    private Dictionary<char, BitList> codesMap;

    public Huffman (string content)
    {

        // ETAP 1 - tu należy zaimplementować tworzenie drzewa Huffmana

        Dictionary<char, int> signs = new Dictionary<char,int>();
        PriorityQueue<Node> queue = new PriorityQueue<Node>(delegate(Node n1, Node n2) { return n1.freq < n2.freq; });

        for (int i = 0; i < content.Length; ++i)
            if (signs.ContainsKey(content[i]))
                signs[content[i]]++;
            else
                signs.Add(content[i], 1);

        foreach (var s in signs)
            queue.Put(new Node(s.Key, s.Value));

        while (queue.Count > 1)
        {
            Node n1 = queue.Get(), n2 = queue.Get();
            queue.Put(new Node(n1.freq + n2.freq, n1, n2));
        }
        root = queue.Get();

        codesMap = new Dictionary<char,BitList>();
        buildCodesMap(root,new BitList());
    }

    private void buildCodesMap(Node node, BitList code)
    {

        // ETAP 2  - tu należy zaimplementować generowanie kodów poszczególnych znaków oraz wypełnianie mapy codesMap

        if (node.character != '\0')
        {
            if (code.Count == 0)
                code.Append(false);
            codesMap.Add(node.character, code);
        }
        else
        {
            BitList leftList = new BitList(code);
            leftList.Append(false);
            buildCodesMap(node.left, leftList);
            BitList rightList = new BitList(code);
            rightList.Append(true);
            buildCodesMap(node.right, rightList);
        }
    }

    public BitList Compress(string content)
    {

        // ETAP 2 - wykorzystując dane w codesMap należy zakodować napis przekazany jako parametr content

        BitList result = new BitList();
        for(int i = 0; i < content.Length; ++i)
            result.Append(codesMap[content[i]]);
        return result;
    }

    public string Decompress(BitList compressed)
    {

        // ETAP 3 - należy zwrócić zdekodowane dane

        Node current = root;
        StringBuilder sbuilder = new StringBuilder();

        if (root.character != '\0')
        {
            for (int i = 0; i < compressed.Count; ++i)
                sbuilder.Append(root.character);
            return sbuilder.ToString();
        }

        for (int i = 0; i < compressed.Count; ++i)
        {
            if (!compressed[i])
                current = current.left;
            else
                current = current.right;
            if (current.character != '\0')
            {
                sbuilder.Append(current.character);
                current = root;
            }
        }

        return sbuilder.ToString();
    }

    }

}
