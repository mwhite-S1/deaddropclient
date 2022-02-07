
/**
 * Created for Dead Drop
 * Date: 2013-10-08
 * Time: 10:28 AM
 */

 var pw;
 var root;
 var domain;
 
 function makePwd() {
     "use strict";
 

     for (var i = 0; i < 5; i++) {
         //throw away a couple
         sjcl.random.randomWords(1);
     }
 
     var m = new MersenneTwister(sjcl.random.randomWords(1));
 
     var text = "";
     var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
 
     for (var i = 0; i < 15; i++) {
         text += possible.charAt(Math.floor(m.random() * possible.length));
     }
 
     return text;
 }
 
 function symmetricEncrypt(pw, message) {
     try {
         "use strict";
         var crypt = sjcl.encrypt(pw, message);
 
         return crypt;
     } catch (err) {
         alert('Error encrypting data');
         return false;
     }
 }
 
 function symmetricDecrypt(pw, data) {
    try {
        "use strict";

        return sjcl.decrypt(pw, data);

    } catch (err) {
        return err.stack;
    }
}
