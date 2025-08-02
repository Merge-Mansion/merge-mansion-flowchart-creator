using QuickGraph.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MergeMansion
{
    public partial class ImageProcessor : Form
    {
        private ListBox categoryListBox;
        private PictureBox mainPictureBox;
        private ListView croppedImagesListView;
        private Button loadCategoryButton;
        private Button exportButton;
        private NumericUpDown topInput, leftInput, widthInput, heightInput, topGapInput, leftGapInput;
        private Label topLabel, leftLabel, widthLabel, heightLabel, topGapLabel, leftGapLabel;
        private ImageList imageList;
        private string path_to_categories_folder = "path_to_categories_folder"; // Replace with your categories folder path

        public ImageProcessor()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            // Set up the form
            this.Text = "Image Processor";
            this.Size = new Size(1000, 800);

            // Set up the category list box
            categoryListBox = new ListBox();
            categoryListBox.Location = new Point(10, 10);
            categoryListBox.Size = new Size(200, 300);
            this.Controls.Add(categoryListBox);

            // Set up the main picture box
            mainPictureBox = new PictureBox();
            mainPictureBox.Location = new Point(220, 10);
            mainPictureBox.Size = new Size(500, 500);
            mainPictureBox.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(mainPictureBox);

            // Set up the cropped images list view
            croppedImagesListView = new ListView();
            croppedImagesListView.Location = new Point(730, 10);
            croppedImagesListView.Size = new Size(250, 500);
            croppedImagesListView.View = View.LargeIcon;
            this.Controls.Add(croppedImagesListView);

            // Set up the load category button
            loadCategoryButton = new Button();
            loadCategoryButton.Text = "Load Category";
            loadCategoryButton.Location = new Point(10, 320);
            loadCategoryButton.Size = new Size(200, 30);
            loadCategoryButton.Click += LoadCategoryButton_Click;
            this.Controls.Add(loadCategoryButton);

            // Set up the export button
            exportButton = new Button();
            exportButton.Text = "Export Cropped Images";
            exportButton.Location = new Point(10, 700);
            exportButton.Size = new Size(200, 30);
            exportButton.Click += ExportButton_Click;
            this.Controls.Add(exportButton);

            // Set up the numeric inputs for cropping
            topLabel = new Label { Text = "Top", Location = new Point(10, 360), Size = new Size(60, 20) };
            this.Controls.Add(topLabel);
            topInput = new NumericUpDown { Location = new Point(70, 360), Size = new Size(140, 20) };
            this.Controls.Add(topInput);

            leftLabel = new Label { Text = "Left", Location = new Point(10, 390), Size = new Size(60, 20) };
            this.Controls.Add(leftLabel);
            leftInput = new NumericUpDown { Location = new Point(70, 390), Size = new Size(140, 20) };
            this.Controls.Add(leftInput);

            widthLabel = new Label { Text = "Width", Location = new Point(10, 420), Size = new Size(60, 20) };
            this.Controls.Add(widthLabel);
            widthInput = new NumericUpDown { Location = new Point(70, 420), Size = new Size(140, 20) };
            this.Controls.Add(widthInput);

            heightLabel = new Label { Text = "Height", Location = new Point(10, 450), Size = new Size(60, 20) };
            this.Controls.Add(heightLabel);
            heightInput = new NumericUpDown { Location = new Point(70, 450), Size = new Size(140, 20) };
            this.Controls.Add(heightInput);

            topGapLabel = new Label { Text = "Top Gap", Location = new Point(10, 480), Size = new Size(60, 20) };
            this.Controls.Add(topGapLabel);
            topGapInput = new NumericUpDown { Location = new Point(70, 480), Size = new Size(140, 20) };
            this.Controls.Add(topGapInput);

            leftGapLabel = new Label { Text = "Left Gap", Location = new Point(10, 510), Size = new Size(60, 20) };
            this.Controls.Add(leftGapLabel);
            leftGapInput = new NumericUpDown { Location = new Point(70, 510), Size = new Size(140, 20) };
            this.Controls.Add(leftGapInput);

            // Add a button to generate crop lines
            Button generateLinesButton = new Button
            {
                Text = "Generate Lines",
                Location = new Point(10, 540),
                Size = new Size(200, 30)
            };
            generateLinesButton.Click += GenerateLinesButton_Click;
            this.Controls.Add(generateLinesButton);
        }

        private void LoadCategoryButton_Click(object sender, EventArgs e)
        {
            // Load categories from the specified folder
            LoadCategories();
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            // Export the cropped images
            ExportCroppedImages();
        }

        private void GenerateLinesButton_Click(object sender, EventArgs e)
        {
            // Generate crop lines based on input values
            GenerateCropLines();
            // Display cropped images in the list view
            DisplayCroppedImages();
        }

        private void LoadCategories()
        {
            categoryListBox.Items.Clear();
            foreach (string categoryPath in Directory.GetFiles(path_to_categories_folder, "*.jpg"))
            {
                string categoryName = Path.GetFileNameWithoutExtension(categoryPath);
                string categorySubFolder = Path.Combine(path_to_categories_folder, categoryName);
                if (Directory.Exists(categorySubFolder))
                {
                    categoryName += " (done)";
                }
                categoryListBox.Items.Add(categoryName);
            }
        }

        private void LoadCategoryImage(string category)
        {
            string imagePath = Path.Combine(path_to_categories_folder, $"{category}.jpg");
            if (File.Exists(imagePath))
            {
                mainPictureBox.Image = Image.FromFile(imagePath);
            }
        }

        private void GenerateCropLines()
        {
            // Generate lines for cropping based on input values
            int top = (int)topInput.Value;
            int left = (int)leftInput.Value;
            int width = (int)widthInput.Value;
            int height = (int)heightInput.Value;
            int topGap = (int)topGapInput.Value;
            int leftGap = (int)leftGapInput.Value;

            // Add code to draw lines on the picture box or use a temporary bitmap to show the crop areas...
        }
        private void DisplayCroppedImages()
        {
            croppedImagesListView.Items.Clear();
            imageList.Images.Clear();

            if (mainPictureBox.Image == null) return;

            int top = (int)topInput.Value;
            int left = (int)leftInput.Value;
            int width = (int)widthInput.Value;
            int height = (int)heightInput.Value;
            int topGap = (int)topGapInput.Value;
            int leftGap = (int)leftGapInput.Value;

            Bitmap mainBitmap = new Bitmap(mainPictureBox.Image);
            int index = 1;
            for (int row = 0; row < 4; row++) // Adjust to 4 rows
            {
                for (int col = 0; col < 4; col++) // Adjust to 4 columns
                {
                    int x = left + (col * (width + leftGap));
                    int y = top + (row * (height + topGap));

                    if (x + width > mainBitmap.Width || y + height > mainBitmap.Height)
                        continue;

                    Rectangle cropRect = new Rectangle(x, y, width, height);
                    Bitmap croppedImage = mainBitmap.Clone(cropRect, mainBitmap.PixelFormat);

                    string category = categoryListBox.SelectedItem.ToString().Replace(" (done)", ""); // Remove " (done)"
                    string key = $"{category}_{index:D2}";
                    imageList.Images.Add(key, croppedImage);
                    croppedImagesListView.Items.Add(new ListViewItem(key, imageList.Images.Count - 1));
                    index++;
                }
            }
        }
        private void ExportCroppedImages()
        {
            string category = categoryListBox.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(category)) return;

            category = category.Replace(" (done)", ""); // Remove " (done)" from the category name

            string outputFolder = Path.Combine(path_to_categories_folder, category); // Use Categories folder
            Directory.CreateDirectory(outputFolder);

            int index = 1;
            foreach (ListViewItem item in croppedImagesListView.Items)
            {
                Image image = imageList.Images[item.ImageIndex];
                string outputImagePath = Path.Combine(outputFolder, $"{category}_{index:D2}.png");
                image.Save(outputImagePath);
                index++;
            }

            MessageBox.Show("Cropped images exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
