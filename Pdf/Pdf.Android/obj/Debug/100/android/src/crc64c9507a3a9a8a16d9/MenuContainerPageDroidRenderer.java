package crc64c9507a3a9a8a16d9;


public class MenuContainerPageDroidRenderer
	extends crc643f46942d9dd1fff9.PageRenderer
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onLayout:(ZIIII)V:GetOnLayout_ZIIIIHandler\n" +
			"n_onSizeChanged:(IIII)V:GetOnSizeChanged_IIIIHandler\n" +
			"";
		mono.android.Runtime.register ("SlideOverKit.Droid.MenuContainerPageDroidRenderer, SlideOverKit.Droid", MenuContainerPageDroidRenderer.class, __md_methods);
	}


	public MenuContainerPageDroidRenderer (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == MenuContainerPageDroidRenderer.class)
			mono.android.TypeManager.Activate ("SlideOverKit.Droid.MenuContainerPageDroidRenderer, SlideOverKit.Droid", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public MenuContainerPageDroidRenderer (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == MenuContainerPageDroidRenderer.class)
			mono.android.TypeManager.Activate ("SlideOverKit.Droid.MenuContainerPageDroidRenderer, SlideOverKit.Droid", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public MenuContainerPageDroidRenderer (android.content.Context p0)
	{
		super (p0);
		if (getClass () == MenuContainerPageDroidRenderer.class)
			mono.android.TypeManager.Activate ("SlideOverKit.Droid.MenuContainerPageDroidRenderer, SlideOverKit.Droid", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public void onLayout (boolean p0, int p1, int p2, int p3, int p4)
	{
		n_onLayout (p0, p1, p2, p3, p4);
	}

	private native void n_onLayout (boolean p0, int p1, int p2, int p3, int p4);


	public void onSizeChanged (int p0, int p1, int p2, int p3)
	{
		n_onSizeChanged (p0, p1, p2, p3);
	}

	private native void n_onSizeChanged (int p0, int p1, int p2, int p3);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
