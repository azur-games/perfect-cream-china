#import "UnityAppController.h"
#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

extern UIViewController *UnityGetGLViewController();

@interface NSMutableAttributedString (SetAsLinnkSupport)
- (BOOL)setAsLink:(NSString*)textToFind linkURL:(NSString*)linkURL;
@end

@implementation NSMutableAttributedString (SetAsLinnkSupport) - (BOOL)setAsLink:(NSString*)textToFind linkURL:(NSString*)linkURL {
     NSRange foundRange = [self.mutableString rangeOfString:textToFind];
     if (foundRange.location != NSNotFound) {
         [self addAttribute:NSLinkAttributeName value:linkURL range:foundRange];
         [self addAttribute:NSUnderlineColorAttributeName value:UIColor.systemBlueColor range:foundRange];
         return YES;
     }
    return NO;
}
@end
    
@interface iOSBridge : NSObject

@end

@implementation iOSBridge
+(void)alertConfirmationView:(NSString*)title addMessage:(NSString*)message addCallBack:(NSString*)callback okButton:(NSString*)okText termsString:(NSString*)termsString policyString:(NSString*)policyString termsURL:(NSString*)termsURL policyURL:(NSString*)policyURL
{
    NSMutableAttributedString *attributedButtonTerms = [[NSMutableAttributedString alloc] initWithString:termsString];
    NSMutableAttributedString *attributedButtonPrivacy = [[NSMutableAttributedString alloc] initWithString:policyString];
    
    BOOL firstLinkWasSet = [attributedButtonTerms setAsLink:termsString linkURL:termsURL];
    BOOL secondLinkWasSet = [attributedButtonPrivacy setAsLink:policyString linkURL:policyURL];

    UIAlertController *alert = [UIAlertController alertControllerWithTitle:title
                                                                   message:message preferredStyle:UIAlertControllerStyleAlert];
 
    UIAlertAction *okAction = [UIAlertAction actionWithTitle:okText style:UIAlertActionStyleDefault
                                                        handler:^(UIAlertAction *action){
                                                            UnitySendMessage("iOSBridgeListener", [callback UTF8String], "");
                                                        }];
   NSURL *tUrl = [NSURL URLWithString:termsURL];
   UIAlertAction *termsAction = [UIAlertAction actionWithTitle:termsString style:UIAlertActionStyleDefault
                  handler:^(UIAlertAction *action){
                        UIApplication *app = [UIApplication sharedApplication];
                            if ([app canOpenURL:tUrl])
                                 {
                                   [[UIApplication sharedApplication] openURL: tUrl options:@{} completionHandler:^(BOOL success) {
                                              [self alertConfirmationView:title addMessage:message
                                                            addCallBack:callback okButton:okText termsString:termsString policyString:policyString termsURL:termsURL policyURL:policyURL];
                                        }];
                                        }
                                    }];
                                                     
    NSURL *pUrl = [NSURL URLWithString:policyURL];
    UIAlertAction *privacyAction = [UIAlertAction actionWithTitle:policyString style:UIAlertActionStyleDefault
                   handler:^(UIAlertAction *action){
                         UIApplication *app = [UIApplication sharedApplication];
                             if ([app canOpenURL:pUrl])
                                  {
                                    [[UIApplication sharedApplication] openURL: pUrl options:@{} completionHandler:^(BOOL success) {
                                               [self alertConfirmationView:title addMessage:message
                                                             addCallBack:callback okButton:okText termsString:termsString policyString:policyString termsURL:termsURL policyURL:policyURL];
                                         }];
                                         }
                                     }];
    
    [alert addAction:termsAction];
    [alert addAction:privacyAction];
    [alert addAction:okAction];
    [UnityGetGLViewController() presentViewController:alert animated:NO completion:nil];
}
@end

char* cStringCopy(const char* string)
{
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    
    return res;
}

extern "C"
{
    void _ShowGDPR(const char *title, const char *message, const char *callBack, const char *okText, char *tTerms, char *tPolicy, char *tURL, char *pURL)
    {
        [iOSBridge alertConfirmationView:[NSString stringWithUTF8String:title] addMessage:[NSString stringWithUTF8String:message]  addCallBack:[NSString stringWithUTF8String:callBack]
                                okButton:[NSString stringWithUTF8String:okText] termsString:[NSString stringWithUTF8String:tTerms] policyString:[NSString stringWithUTF8String:tPolicy] termsURL:[NSString stringWithUTF8String:tURL] policyURL:[NSString stringWithUTF8String:pURL]];
    }
}
