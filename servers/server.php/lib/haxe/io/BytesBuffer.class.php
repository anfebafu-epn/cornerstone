<?php

// Generated by Haxe 3.4.2 (git build master @ 890f8c7)
class haxe_io_BytesBuffer {
	public function __construct() {
		if(!php_Boot::$skip_constructor) {
		$this->b = "";
	}}
	public $b;
	public function get_length() {
		return strlen($this->b);
	}
	public function addByte($byte) {
		$tmp = $this;
		$tmp1 = $tmp->b;
		$tmp->b = _hx_string_or_null($tmp1) . _hx_string_or_null(chr($byte));
	}
	public function add($src) {
		$tmp = $this;
		$tmp->b = _hx_string_or_null($tmp->b) . _hx_string_or_null($src->b->s);
	}
	public function addString($v) {
		$tmp = $this;
		$tmp->b = _hx_string_or_null($tmp->b) . _hx_string_or_null(haxe_io_Bytes::ofString($v)->b->s);
	}
	public function addInt32($v) {
		$tmp = $this;
		$tmp1 = $tmp->b;
		$tmp->b = _hx_string_or_null($tmp1) . _hx_string_or_null(chr($v & 255));
		$tmp2 = $this;
		$tmp3 = $tmp2->b;
		$tmp2->b = _hx_string_or_null($tmp3) . _hx_string_or_null(chr($v >> 8 & 255));
		$tmp4 = $this;
		$tmp5 = $tmp4->b;
		$tmp4->b = _hx_string_or_null($tmp5) . _hx_string_or_null(chr($v >> 16 & 255));
		$tmp6 = $this;
		$tmp7 = $tmp6->b;
		$tmp6->b = _hx_string_or_null($tmp7) . _hx_string_or_null(chr(_hx_shift_right($v, 24)));
	}
	public function addInt64($v) {
		$this->addInt32($v->low);
		$this->addInt32($v->high);
	}
	public function addFloat($v) {
		$this->addInt32(haxe_io_FPHelper::floatToI32($v));
	}
	public function addDouble($v) {
		$this->addInt64(haxe_io_FPHelper::doubleToI64($v));
	}
	public function addBytes($src, $pos, $len) {
		$tmp = null;
		$tmp1 = null;
		if($pos >= 0) {
			$tmp1 = $len < 0;
		} else {
			$tmp1 = true;
		}
		if(!$tmp1) {
			$tmp = $pos + $len > $src->length;
		} else {
			$tmp = true;
		}
		if($tmp) {
			throw new HException(haxe_io_Error::$OutsideBounds);
		}
		$tmp2 = $this;
		$tmp3 = $tmp2->b;
		$this1 = $src->b;
		$x = new php__BytesData_Wrapper(substr($this1->s, $pos, $len));
		$this2 = $x;
		$tmp2->b = _hx_string_or_null($tmp3) . _hx_string_or_null($this2->s);
	}
	public function getBytes() {
		$bytes = strlen($this->b);
		$x = new php__BytesData_Wrapper($this->b);
		$this1 = $x;
		$bytes1 = new haxe_io_Bytes($bytes, $this1);
		$this->b = null;
		return $bytes1;
	}
	public function __call($m, $a) {
		if(isset($this->$m) && is_callable($this->$m))
			return call_user_func_array($this->$m, $a);
		else if(isset($this->__dynamics[$m]) && is_callable($this->__dynamics[$m]))
			return call_user_func_array($this->__dynamics[$m], $a);
		else if('toString' == $m)
			return $this->__toString();
		else
			throw new HException('Unable to call <'.$m.'>');
	}
	static $__properties__ = array("get_length" => "get_length");
	function __toString() { return 'haxe.io.BytesBuffer'; }
}
