<?php

// Generated by Haxe 3.4.2 (git build master @ 890f8c7)
class haxe_io__ArrayBufferView_ArrayBufferView_Impl_ {
	public function __construct(){}
	static function get_EMULATED() {
		return false;
	}
	static function _new($size) {
		$this1 = new haxe_io_ArrayBufferViewImpl(haxe_io_Bytes::alloc($size), 0, $size);
		return $this1;
	}
	static function get_byteOffset($this1) {
		return $this1->byteOffset;
	}
	static function get_byteLength($this1) {
		return $this1->byteLength;
	}
	static function get_buffer($this1) {
		return $this1->bytes;
	}
	static function sub($this1, $begin, $length = null) {
		return $this1->sub($begin, $length);
	}
	static function subarray($this1, $begin = null, $end = null) {
		return $this1->subarray($begin, $end);
	}
	static function getData($this1) {
		return $this1;
	}
	static function fromData($a) {
		return $a;
	}
	static function fromBytes($bytes, $pos = null, $length = null) {
		if($pos === null) {
			$pos = 0;
		}
		if($length === null) {
			$length = $bytes->length - $pos;
		}
		$tmp = null;
		$tmp1 = null;
		if($pos >= 0) {
			$tmp1 = $length < 0;
		} else {
			$tmp1 = true;
		}
		if(!$tmp1) {
			$tmp = $pos + $length > $bytes->length;
		} else {
			$tmp = true;
		}
		if($tmp) {
			throw new HException(haxe_io_Error::$OutsideBounds);
		}
		$a = new haxe_io_ArrayBufferViewImpl($bytes, $pos, $length);
		return $a;
	}
	static $__properties__ = array("get_byteLength" => "get_byteLength","get_byteOffset" => "get_byteOffset","get_buffer" => "get_buffer","get_EMULATED" => "get_EMULATED");
	function __toString() { return 'haxe.io._ArrayBufferView.ArrayBufferView_Impl_'; }
}
