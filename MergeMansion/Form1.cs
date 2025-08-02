using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MergeMansion.MergeType;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using NewtonsoftJson = Newtonsoft.Json.JsonConvert;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using JsonException = System.Text.Json.JsonException;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;


namespace MergeMansion
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            Root root = LoadRootFromFile("JSON/chain_item_odds.json");

            SetupTree(root);

            // Initialize the list to hold tasks with empty items and their corresponding areas
            var allEmptyTasksWithArea = new List<TaskWithArea>();

            // Path to the JSON file containing area data
            string jsonFilePath = "json/areas.json";

            // Read the area data from the JSON file
            AreaCollection areaCollection = JsonReader.ReadJson(jsonFilePath);

            // Initialize the AreaProcessor with the output TextBox
            AreaProcessor processor = new AreaProcessor(root);

            // Process each area and collect tasks with empty items
            foreach (var area in areaCollection.Data)
            {
                var emptyTasksWithArea = processor.ProcessArea(area);
                if (emptyTasksWithArea != null && emptyTasksWithArea.Count > 0)
                {
                    allEmptyTasksWithArea.AddRange(emptyTasksWithArea);
                }
            }

            // Using LINQ to find the DrawerData with Name "Drawer"
            var drawerData = root.Data.FirstOrDefault(d => d.Name == "Drawer");

            if (drawerData != null)
            {
                Debug.WriteLine($"Name: {drawerData.Name}");
                Debug.WriteLine($"ConfigKey: {drawerData.ConfigKey}");

                // Iterate through PrimaryChain and display Name, Description, ItemType
                foreach (var chain in drawerData.PrimaryChain)
                {
                    Debug.WriteLine($"Item Name: {chain.Item.Name}");
                    Debug.WriteLine($"Item Description: {chain.Item.Description}");
                    Debug.WriteLine($"Item Type: {chain.Item.ItemType}");
                }
            }
            else
            {
                Debug.WriteLine("DrawerData with Name 'Drawer' not found.");
            }


            // Display the JSON output in textBox1
            //textBox1.Text = jsonOutput;

        }

        private void SetupTree(Root root)
        {
            // Create an ImageList
            ImageList imageList = new ImageList();
            LoadImages(imageList);

            // Assign ImageList to the TreeView
            treeView1.ImageList = imageList;

            // Lists to store root nodes
            List<TreeNode> nodesWithImages = new List<TreeNode>();
            List<TreeNode> nodesWithoutImages = new List<TreeNode>();

            foreach (var chain in root.Data)
            {
                TreeNode rootNode = new TreeNode(chain.ConfigKey.ToString());

                bool hasImage = false;

                foreach (var subChain in chain.PrimaryChain)
                {
                    if (subChain.Item != null)
                    {
                        TreeNode childNode = new TreeNode(subChain.Item.Name);

                        // Set the image for the node by specifying the key
                        string imageKey = subChain.Item.ItemType;
                        if (imageList.Images.ContainsKey(imageKey))
                        {
                            childNode.ImageKey = imageKey;
                            childNode.SelectedImageKey = imageKey;
                            hasImage = true; // Mark that this root node has a child with an image
                        }
                        rootNode.Nodes.Add(childNode);
                    }
                }

                // Add root nodes to the appropriate list
                if (hasImage)
                {
                    nodesWithImages.Add(rootNode);
                }
                else
                {
                    nodesWithoutImages.Add(rootNode);
                }
            }

            // Sort nodes with images alphabetically
            nodesWithImages.Sort((x, y) => string.Compare(x.Text, y.Text));

            // Add nodes with images first, followed by nodes without images
            foreach (TreeNode node in nodesWithImages)
            {
                treeView1.Nodes.Add(node);
            }
            foreach (TreeNode node in nodesWithoutImages)
            {
                treeView1.Nodes.Add(node);
            }
        }

        private void LoadImages1(ImageList imageList)
        {
            Dictionary<string, int> imageCategories = new Dictionary<string, int>
            {
                { "Bottle", 8 },
                { "MaintenanceTools", 12 }
            };

            imageList.ImageSize = new Size(52, 100);

            // Create the transparent placeholder image
            Bitmap placeholderImage = CreateTransparentPlaceholder(52, 100); // Adjust width and height as needed

            // Add the transparent placeholder image
            imageList.Images.Add("placeholder", placeholderImage);

            // Add images to the ImageList
            foreach (var category in imageCategories)
            {
                string categoryName = category.Key;
                int count = category.Value;

                for (int i = 1; i <= count; i++)
                {
                    string imageKey = $"{categoryName}_{i:D2}";
                    string imagePath = $"Images/{imageKey}.png";

                    // Check if the image file exists before adding
                    if (File.Exists(imagePath))
                    {
                        AddResizedImage(imageList, imageKey, imagePath);
                    }
                }
            }
        }

        private void LoadImages(ImageList imageList)
        {
            Dictionary<string, int> imageCategories = new Dictionary<string, int>
        {
            { "Bottle", 8 },
            { "MaintenanceTools", 12 }
        };

            imageList.ImageSize = new Size(52, 100);

            // Create the transparent placeholder image
            Bitmap placeholderImage = CreateTransparentPlaceholder(52, 100);

            // Add the transparent placeholder image
            imageList.Images.Add("placeholder", placeholderImage);

            // Add images to the ImageList
            foreach (var category in imageCategories)
            {
                string categoryName = category.Key;
                int count = category.Value;

                for (int i = 1; i <= count; i++)
                {
                    string imageKey = $"{categoryName}_{i:D2}";
                    string imagePath = $"Images/{imageKey}.png";

                    // Check if the image file exists before adding
                    if (File.Exists(imagePath))
                    {
                        AddResizedImage(imageList, imageKey, imagePath);
                    }
                }
            }
        }

        private void AddResizedImage(ImageList imageList, string imageKey, string imagePath)
        {
            Image originalImage = Image.FromFile(imagePath);
            Image resizedImage = ResizeImageToFit(imageList.ImageSize, originalImage);
            imageList.Images.Add(imageKey, resizedImage);
        }

        private Image ResizeImageToFit(Size targetSize, Image originalImage)
        {
            Bitmap resizedBitmap = new Bitmap(targetSize.Width, targetSize.Height);
            using (Graphics graphics = Graphics.FromImage(resizedBitmap))
            {
                graphics.Clear(Color.Transparent);
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                // Calculate the position to center the image
                float scale = Math.Min((float)targetSize.Width / originalImage.Width, (float)targetSize.Height / originalImage.Height);
                int width = (int)(originalImage.Width * scale);
                int height = (int)(originalImage.Height * scale);
                int x = (targetSize.Width - width) / 2;
                int y = (targetSize.Height - height) / 2;

                graphics.DrawImage(originalImage, x, y, width, height);
            }

            return resizedBitmap;
        }


        private Bitmap CreateTransparentPlaceholder(int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.Transparent);
            }
            return bitmap;
        }

        public Root LoadRootFromFile(string filePath)
        {
            try
            {
                string jsonString = File.ReadAllText(filePath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter(), new ActivationSpawnConverter(), new ResultProducerConverter() }
                };

                Root root = JsonSerializer.Deserialize<Root>(jsonString, options);
                return root;
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON Error: {ex.Message}");
                Debug.WriteLine($"Path: {ex.Path}");
                Debug.WriteLine($"LineNumber: {ex.LineNumber}");
                Debug.WriteLine($"BytePositionInLine: {ex.BytePositionInLine}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"General Error: {ex.Message}");
                return null;
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Check if the selected node is a root node (no parent)
            if (e.Node.Parent == null)
            {
                // Clear the current view control (e.g., ListView, DataGridView, etc.)
                listView1.Items.Clear();

                // Link the ImageList to the ListView
                listView1.LargeImageList = treeView1.ImageList;

                // Display the children of the selected root node in the view control
                foreach (TreeNode childNode in e.Node.Nodes)
                {
                    // Create a list view item with the child's text and image key
                    ListViewItem item = new ListViewItem(childNode.Text);

                    // Set the image for the item by specifying the key
                    string imageKey = childNode.ImageKey;
                    if (treeView1.ImageList.Images.ContainsKey(imageKey))
                    {
                        item.ImageKey = imageKey;
                    }

                    // Add the item to the view control
                    listView1.Items.Add(item);
                }
            }
        }
    }
}


