package cornerstone.integrator;

import cornerstone.integrator.helpers.Int64_Helper;
import haxe.Int64;
import haxe.io.UInt8Array;
import haxe.Log;

@:keepSub
class LibReference {
    public function new() {
    }

    public function Dummy(String1:String,
                                 Int2:Int,
                                 Float3:Float,
                                 Bool4:Bool,
                                 Byte5:UInt8Array,
                                 DateTime6:Date,
                                 Enum7:Int,
                                 Long8:Int64,
                                 Dictionary9:Map<Int, String>,
                                 //Single10:Single, // NO EXISTENTE EN JS, descartado
                                 Uuid11:String) {
        var data:String = "";
        Type.getClass(Int64_Helper);
        Type.getClass(cornerstone.integrator.datatypes.UUID);
        Log.trace("test", null);
        return null;
    }
}
