package cornerstone.integrator.exceptions;
@:keep
class LogicException extends Exception {

    public function new(Code:Int, Msg:String) {
        this.Code = Code;
        super(Msg);
    }

    @:isVar public var Code(get, set):Int;

    function set_Code(value:Int) {
        return this.Code = value;
    }

    function get_Code():Int {
        return Code;
    }
}
