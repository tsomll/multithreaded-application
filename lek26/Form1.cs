namespace lek26
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            _animator = new Animator(panel1.CreateGraphics());
            _animator.Start();//аниматор будет стартоват с самого начала , а клик будет вызывать адд серкл
        }
        private Animator _animator;
        private void panel1_Click(object sender, EventArgs e)
        {
           _animator.AddCircle();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _animator.Stop();
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            _animator.MainGraphics = panel1.CreateGraphics();
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _animator.AddCircle();
            else
                _animator.ChangePausedState();
        }
    }
}