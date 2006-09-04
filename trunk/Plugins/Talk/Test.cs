using Niry;
using Niry.GUI.Gtk2;

public class Test : ShapedWindow {
	public Test () : base(new Gdk.Pixbuf("Pixmaps/TalkPopup.xpm")) {
		Gtk.Fixed fixedBox = new Gtk.Fixed();
		Add(fixedBox);

		fixedBox.Put(new Gtk.Label("Prova"), 100, 100);
		this.ShowAll();
	}

	public static void Main() {
		Gtk.Application.Init();
		new Test();
		Gtk.Application.Run();
	}
}
