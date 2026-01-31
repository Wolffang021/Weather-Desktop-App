namespace Weather_Desktop_App;

public partial class Form1 : Form
{
    TextBox cityInput;
    Button searchButton;

    public Form1()
    {
        InitializeComponent();
    }

    private void SearchUsingEnter(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            Search(sender, (EventArgs)e);
        }
    }

    private void Search(object sender, EventArgs e)
    {
        Console.WriteLine(cityInput.Text);
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        cityInput = new TextBox
        {
            Location = new Point(300, 40),
            Size = new Size(182, 20),
            PlaceholderText = "Enter city..."
        };
        cityInput.KeyUp += SearchUsingEnter;
        this.Controls.Add(cityInput);

        searchButton = new Button
        {
            Location = new Point(482, 38),
            Size = new Size(30, 30),
            Text = "🔎",
            BackColor = System.Drawing.SystemColors.Control
        };
        searchButton.Click += Search;
        this.Controls.Add(searchButton);

        Console.Write(cityInput.Size.Height);
    }
}
