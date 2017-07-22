<?php

// Generated by Haxe 3.4.2 (git build master @ 890f8c7)
class haxe_io_BytesInput extends haxe_io_Input {
	public function __construct($b, $pos = null, $len = null) {
		if(!php_Boot::$skip_constructor) {
		if($pos === null) {
			$pos = 0;
		}
		if($len === null) {
			$len = $b->length - $pos;
		}
		$tmp = null;
		$tmp1 = null;
		if($pos >= 0) {
			$tmp1 = $len < 0;
		} else {
			$tmp1 = true;
		}
		if(!$tmp1) {
			$tmp = $pos + $len > $b->length;
		} else {
			$tmp = true;
		}
		if($tmp) {
			throw new HException(haxe_io_Error::$OutsideBounds);
		}
		$this->b = $b->b;
		$this->pos = $pos;
		$this->len = $len;
		$this->totlen = $len;
	}}
	public $b;
	public $pos;
	public $len;
	public $totlen;
	public function get_position() {
		return $this->pos;
	}
	public function get_length() {
		return $this->totlen;
	}
	public function set_position($p) {
		if($p < 0) {
			$p = 0;
		} else {
			if($p > $this->totlen) {
				$p = $this->totlen;
			}
		}
		$this->len = $this->totlen - $p;
		return $this->pos = $p;
	}
	public function readByte() {
		if($this->len === 0) {
			throw new HException(new haxe_io_Eof());
		}
		$this->len--;
		$this1 = $this->b;
		$pos = $this->pos++;
		return ord($this1->s[$pos]);
	}
	public function readBytes($buf, $pos, $len) {
		$tmp = null;
		$tmp1 = null;
		if($pos >= 0) {
			$tmp1 = $len < 0;
		} else {
			$tmp1 = true;
		}
		if(!$tmp1) {
			$tmp = $pos + $len > $buf->length;
		} else {
			$tmp = true;
		}
		if($tmp) {
			throw new HException(haxe_io_Error::$OutsideBounds);
		}
		$tmp2 = null;
		if($this->len === 0) {
			$tmp2 = $len > 0;
		} else {
			$tmp2 = false;
		}
		if($tmp2) {
			throw new HException(new haxe_io_Eof());
		}
		if($this->len < $len) {
			$len = $this->len;
		}
		{
			$this1 = $buf->b;
			$src = $this->b;
			$srcpos = $this->pos;
			$this1->s = substr($this1->s, 0, $pos) . substr($src->s, $srcpos, $len) . substr($this1->s, $pos+$len);
		}
		$tmp3 = $this;
		$tmp3->pos = $tmp3->pos + $len;
		$tmp4 = $this;
		$tmp4->len = $tmp4->len - $len;
		return $len;
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
	static $__properties__ = array("get_length" => "get_length","set_position" => "set_position","get_position" => "get_position","set_bigEndian" => "set_bigEndian");
	function __toString() { return 'haxe.io.BytesInput'; }
}