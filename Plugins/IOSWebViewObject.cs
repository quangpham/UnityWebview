using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Callback = System.Action<string>;

public class IOSWebViewObject : MonoBehaviour
{
	Callback callback;
	IntPtr webView;

	[DllImport("__Internal")]
	private static extern IntPtr _WebViewPlugin_InitWithMargins(string gameObject, int left, int top, int right, int bottom);
	[DllImport("__Internal")]
	private static extern int _WebViewPlugin_Destroy(IntPtr instance);
	[DllImport("__Internal")]
	private static extern void _WebViewPlugin_SetVisibility(
		IntPtr instance, bool visibility, bool animate);
	[DllImport("__Internal")]
	private static extern void _WebViewPlugin_LoadURL(
		IntPtr instance, string url);
	[DllImport("__Internal")]
	private static extern void _WebViewPlugin_EvaluateJS(
		IntPtr instance, string url);
	
	public void InitWithMargins(int left, int top, int right, int bottom)
	{
		webView = _WebViewPlugin_InitWithMargins(name, left, top, right, bottom);
		/*if (webView != IntPtr.Zero) 
			_WebViewPlugin_LoadURL(webView, webUrl);
			*/
	}

	void OnDestroy()
	{
		if (webView == IntPtr.Zero)
			return;
		_WebViewPlugin_Destroy(webView);
	}

	public void SetVisibility(bool v, Callback cb = null)
	{
		if (webView == IntPtr.Zero)
			return;
		callback = cb;
		// private static extern void _WebViewPlugin_SetVisibility(IntPtr instance, bool visibility, bool animate);
		_WebViewPlugin_SetVisibility(webView, v, true);
	}

	public void LoadURL(string url)
	{
		if (webView == IntPtr.Zero)
			return;
		_WebViewPlugin_LoadURL(webView, url);
	}

	public void EvaluateJS(string js)
	{
		if (webView == IntPtr.Zero)
			return;
		_WebViewPlugin_EvaluateJS(webView, js);
	}

	public void CallFromJS(string message)
	{
		if (callback != null)
			callback(message);
	}
}
