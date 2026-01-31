namespace Weather_Desktop_App;

public partial class Form1 : Form
{
    TextBox cityInput;
    Button searchButton;

    public Form1()
    {
        InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        cityInput = new TextBox
        {
            Location = new Point(300, 40),
            Size = new Size(182, 20),
            PlaceholderText = "Enter city..."
        };
        this.Controls.Add(cityInput);

        searchButton = new Button
        {
            Location = new Point(482, 40),
            Size = new Size(28, 28),
            Text = "🔎"
        };
        this.Controls.Add(searchButton);

        Console.Write(cityInput.Size.Height);
    }
}
