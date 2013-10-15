#import <UIKit/UIKit.h>

extern UIViewController *UnityGetGLViewController();
extern "C" void UnitySendMessage(const char *, const char *, const char *);

@interface WebViewPlugin : NSObject<UIWebViewDelegate, UIScrollViewDelegate>
{
    UIScrollView *containerView;
    CGRect frame;
    //NSString *currentURL;
	UIWebView *webView;
	NSString *gameObjectName;
}
@end

@implementation WebViewPlugin

- (id)initWithGameObjectNameAndMargin:(const char *)gameObjectName_ left:(int)left top:(int)top right:(int)right bottom:(int)bottom
{
	self = [super init];
    
	UIView *view = UnityGetGLViewController().view;
    [view setBackgroundColor: [UIColor clearColor]];
    [view setOpaque:NO];
    
    frame = view.frame;
	CGFloat scale = view.contentScaleFactor;
	frame.size.width -= (left + right) / scale;
	frame.size.height -= (top + bottom) / scale;
	frame.origin.x += left / scale;
	frame.origin.y += top / scale;
    
    containerView = [[UIScrollView alloc] initWithFrame:frame];
    containerView.delegate = self;
    containerView.contentSize = CGSizeMake(frame.size.width, frame.size.height * 2);
    [containerView setBackgroundColor: [UIColor clearColor]];
    [containerView setScrollEnabled:NO];
    [view addSubview:containerView];
    
	webView = [[UIWebView alloc] initWithFrame:CGRectMake(frame.origin.x , frame.size.height, frame.size.width, frame.size.height)];
	webView.delegate = self;
    [containerView addSubview:webView];
    
	gameObjectName = [[NSString stringWithUTF8String:gameObjectName_] retain];
    
	return self;
}

- (void) showWebviewWithAnimation:(BOOL)animate
{
    [containerView setContentOffset:CGPointMake(0, frame.size.height) animated:animate];
}

- (void) hideWebviewWithAnimation:(BOOL)animate
{
    [containerView setContentOffset:CGPointMake(0, 0) animated:animate];
}

- (void)scrollViewDidEndScrollingAnimation:(UIScrollView *)scrollView
{
    if (CGPointEqualToPoint(containerView.contentOffset, CGPointMake(0, 0))) {
        UnitySendMessage([gameObjectName UTF8String], "CallFromJS", "AnimateOff");
    } else if (CGPointEqualToPoint(containerView.contentOffset, CGPointMake(0, frame.size.height))) {
        UnitySendMessage([gameObjectName UTF8String], "CallFromJS", "AnimateOn");
    }
    
}

- (void)dealloc
{
	[webView removeFromSuperview];
	[webView release];
    [containerView removeFromSuperview];
    [containerView release];
    //[currentURL release];
	[gameObjectName release];
	[super dealloc];
}


- (BOOL)webView:(UIWebView *)webView shouldStartLoadWithRequest:(NSURLRequest *)request navigationType:(UIWebViewNavigationType)navigationType
{
	/*
     NSString *url = [[request URL] absoluteString];
     if ([url hasPrefix:@"unity:"]) {
     UnitySendMessage([gameObjectName UTF8String], "CallFromJS", [[url substringFromIndex:6] UTF8String]);
     return NO;
     } else {
     return YES;
     }*/
    NSURL *url = [request URL];
    if ([[url scheme] isEqualToString:@"wvjbscheme"]) {
        if ([[url host] isEqualToString:@"__WVJB_QUEUE_MESSAGE__"]) {
            [self _flushMessageQueue];
        } else {
            NSLog(@"WebViewJavascriptBridge: WARNING: Received unknown WebViewJavascriptBridge command %@://%@", @"wvjbscheme", [url path]);
        }
        return NO;
    }
}

- (void)_flushMessageQueue {
    NSString *messageQueueString = [webView stringByEvaluatingJavaScriptFromString:@"WebViewJavascriptBridge._fetchQueue();"];
    NSLog(@"Log message %@", messageQueueString);
}
/*
 - (void)webViewDidFinishLoad:(UIWebView *)webView
 {
 NSLog(@"Webview load done");
 }
 */
- (void)setVisibility:(BOOL)visibility withAnimation:(BOOL)animate
{
	if (visibility) {
        [self showWebviewWithAnimation:animate];
    } else {
        [self hideWebviewWithAnimation:animate];
    }
}

- (void)loadURL:(const char *)url
{
    /*
     if (currentURL != nil) {
     
     }
     */
    [webView stringByEvaluatingJavaScriptFromString:@"document.open();document.close()"];
	NSString *currentURL = [NSString stringWithUTF8String:url];
	NSURL *nsurl = [NSURL URLWithString:@"https://dl.dropboxusercontent.com/u/14181582/_temp/unitywebview/ExampleApp.html"];
	NSURLRequest *request = [NSURLRequest requestWithURL:nsurl];
	[webView loadRequest:request];
}

- (void)evaluateJS:(const char *)js
{
	NSString *jsStr = [NSString stringWithUTF8String:js];
	[webView stringByEvaluatingJavaScriptFromString:jsStr];
}

@end

extern "C" {
    void *_WebViewPlugin_InitWithMargins(const char *gameObjectName, int left, int top, int right, int bottom);
	void _WebViewPlugin_Destroy(void *instance);
	void _WebViewPlugin_SetMargins(void *instance, int left, int top, int right, int bottom);
	void _WebViewPlugin_SetVisibility(void *instance, BOOL visibility, BOOL animate);
	void _WebViewPlugin_LoadURL(void *instance, const char *url);
	void _WebViewPlugin_EvaluateJS(void *instance, const char *url);
}

void *_WebViewPlugin_InitWithMargins(const char *gameObjectName, int left, int top, int right, int bottom)
{
	id instance = [[WebViewPlugin alloc] initWithGameObjectNameAndMargin:gameObjectName left:left top:top right:right bottom:bottom];
	return (void *)instance;
}

void _WebViewPlugin_Destroy(void *instance)
{
	WebViewPlugin *webViewPlugin = (WebViewPlugin *)instance;
	[webViewPlugin release];
}

void _WebViewPlugin_SetVisibility(void *instance, BOOL visibility, BOOL animate)
{
	WebViewPlugin *webViewPlugin = (WebViewPlugin *)instance;
	[webViewPlugin setVisibility:visibility withAnimation:animate];
}

void _WebViewPlugin_LoadURL(void *instance, const char *url)
{
	WebViewPlugin *webViewPlugin = (WebViewPlugin *)instance;
	[webViewPlugin loadURL:url];
}

void _WebViewPlugin_EvaluateJS(void *instance, const char *js)
{
	WebViewPlugin *webViewPlugin = (WebViewPlugin *)instance;
	[webViewPlugin evaluateJS:js];
}
