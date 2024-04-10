using System;
using System.IO;
using UnityEngine;
using YamlDotNet.RepresentationModel;

public class YamlTester : MonoBehaviour
{
    void Start()
    {
        // Load the YAML file

        if (!File.Exists("myfile.yaml"))
        {
            Debug.Log("Could not find YAML file to test");
            return;
        }

        using (var reader = new StreamReader("myfile.yaml"))
        {
            YamlStream yamlStream = new YamlStream();
            yamlStream.Load(reader);

            // Get the root node
            YamlMappingNode rootNode = (YamlMappingNode)yamlStream.Documents[0].RootNode;

            // Access values in the YAML file

            // === SCALAR ===
            string title = rootNode["title"].ToString();
            string name = rootNode["objects"]["player"]["name"].ToString();
            float health = float.Parse(rootNode["objects"]["player"]["health"].ToString());
            int numEnemies = int.Parse(rootNode["objects"]["enemies"]["count"].ToString());

            // === CHECK EXISTANCE ===
            YamlMappingNode player = (YamlMappingNode)rootNode["objects"]["player"];
            if (!player.Children.ContainsKey("name"))
            {
                Debug.Log("No Player Age found");
            }


            // === ITERATE SQUENCE ===
            YamlSequenceNode valListNode = (YamlSequenceNode)rootNode["val-list"];
            int[] valList = new int[valListNode.Children.Count];

            for (int i = 0; i < valListNode.Children.Count; i++)
            {
                valList[i] = int.Parse(valListNode.Children[i].ToString());
            }

            // Output all values to console
            Debug.Log($"Title: {title}");
            Debug.Log($"Name: {name}");
            Debug.Log($"Health: {health}");
            Debug.Log($"Number of Enemies: {numEnemies}");
            Debug.Log("Values:");
            foreach (int i in valList)
            {
                Debug.Log($"    {i}");
            }

        }

        // === OUTPUT ===

        YamlMappingNode root = new YamlMappingNode();
        root.Add("title", "My Game");

        YamlMappingNode objects = new YamlMappingNode();
        YamlMappingNode player_out = new YamlMappingNode();

        player_out.Add("name", "Xepherin");
        player_out.Add("health", 100f.ToString());
        objects.Add("player", player_out);

        YamlMappingNode enemies = new YamlMappingNode();
        enemies.Add("count", 25.ToString());
        objects.Add("enemies", enemies);

        root.Add("objects", objects);

        YamlSequenceNode val_list = new YamlSequenceNode();
        val_list.Add(new YamlScalarNode(20.ToString()));
        val_list.Add(new YamlScalarNode(35.ToString()));
        val_list.Add(new YamlScalarNode(99.ToString()));
        root.Add("val-list", val_list);


        YamlStream yamlStream_out = new YamlStream();

        using (var writer = new StreamWriter("myfile_out.yaml"))
        {
            yamlStream_out.Add(new YamlDocument(root));
            // Removing the "..." from end of file
            using (var sw = new StringWriter())
            {
                yamlStream_out.Save(sw, false);
                writer.Write(sw.ToString().Substring(0, sw.ToString().Length - 5));
            }

            //yamlStream.Save(writer, assignAnchors: false);
            //yamlStream_out.Save(writer, false);
        }
    }
}
