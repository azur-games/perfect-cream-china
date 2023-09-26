#import "UnityAppController.h"
#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

extern UIViewController *UnityGetGLViewController();

@interface iOSBridge : NSObject

@end

@implementation iOSBridge
+(void)alertConfirmationView:(NSString*)title addMessage:(NSString*)message addCallBack:(NSString*)callback okButton:(NSString*)okText
{
    UIAlertController *alert = [UIAlertController alertControllerWithTitle:title
                                                                   message:message preferredStyle:UIAlertControllerStyleAlert];
    
    UIAlertAction *okAction = [UIAlertAction actionWithTitle:okText style:UIAlertActionStyleDefault
                                                     handler:^(UIAlertAction *action){
                                                         UnitySendMessage("iOSBridge", [callback UTF8String], "");
                                                     }];
    
    
    [alert addAction:okAction];
    [UnityGetGLViewController() presentViewController:alert animated:YES completion:nil];
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
    void _ShowGDPR(const char *title, const char *message, const char *callBack, const char *okText)
    {
        [iOSBridge alertConfirmationView:[NSString stringWithUTF8String:title] addMessage:[NSString stringWithUTF8String:message]  addCallBack:[NSString stringWithUTF8String:callBack] okButton:[NSString stringWithUTF8String:okText]];
    }
}
