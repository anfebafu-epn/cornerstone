<?php

// Generated by Haxe 3.4.2 (git build master @ 890f8c7)
class haxe_io_Bytes {
	public function __construct($length, $b) {
		if(!php_Boot::$skip_constructor) {
		$this->length = $length;
		$this->b = $b;
	}}
	public $length;
	public $b;
	public function get($pos) {
		$this1 = $this->b;
		return ord($this1->s[$pos]);
	}
	public function set($pos, $v) {
		$this1 = $this->b;
		$this1->s[$pos] = chr($v);
	}
	public function blit($pos, $src, $srcpos, $len) {
		$tmp = null;
		$tmp1 = null;
		$tmp2 = null;
		$tmp3 = null;
		if($pos >= 0) {
			$tmp3 = $srcpos < 0;
		} else {
			$tmp3 = true;
		}
		if(!$tmp3) {
			$tmp2 = $len < 0;
		} else {
			$tmp2 = true;
		}
		if(!$tmp2) {
			$tmp1 = $pos + $len > $this->length;
		} else {
			$tmp1 = true;
		}
		if(!$tmp1) {
			$tmp = $srcpos + $len > $src->length;
		} else {
			$tmp = true;
		}
		if($tmp) {
			throw new HException(haxe_io_Error::$OutsideBounds);
		}
		{
			$this1 = $this->b;
			$src1 = $src->b;
			$this1->s = substr($this1->s, 0, $pos) . substr($src1->s, $srcpos, $len) . substr($this1->s, $pos+$len);
		}
	}
	public function fill($pos, $len, $value) {
		$_g1 = 0;
		$_g = $len;
		while($_g1 < $_g) {
			$_g1 = $_g1 + 1;
			$i = $_g1 - 1;
			{
				$pos = $pos + 1;
				$pos1 = $pos - 1;
				{
					$this1 = $this->b;
					$this1->s[$pos1] = chr($value);
					unset($this1);
				}
				unset($pos1);
			}
			unset($i);
		}
	}
	public function sub($pos, $len) {
		$tmp = null;
		$tmp1 = null;
		if($pos >= 0) {
			$tmp1 = $len < 0;
		} else {
			$tmp1 = true;
		}
		if(!$tmp1) {
			$tmp = $pos + $len > $this->length;
		} else {
			$tmp = true;
		}
		if($tmp) {
			throw new HException(haxe_io_Error::$OutsideBounds);
		}
		$this1 = $this->b;
		$x = new php__BytesData_Wrapper(substr($this1->s, $pos, $len));
		$this2 = $x;
		return new haxe_io_Bytes($len, $this2);
	}
	public function compare($other) {
		$this1 = $this->b;
		$other1 = $other->b;
		return $this1->s < $other1->s ? -1 : ($this1->s == $other1->s ? 0 : 1);
	}
	public function getDouble($pos) {
		$this1 = $this->b;
		$v = ord($this1->s[$pos]);
		$this2 = $this->b;
		$v1 = $v | ord($this2->s[$pos + 1]) << 8;
		$this3 = $this->b;
		$v2 = $v1 | ord($this3->s[$pos + 2]) << 16;
		$this4 = $this->b;
		$v3 = $v2 | ord($this4->s[$pos + 3]) << 24;
		$tmp = null;
		if(($v3 & -2147483648) !== 0) {
			$tmp = $v3 | -2147483648;
		} else {
			$tmp = $v3;
		}
		$pos1 = $pos + 4;
		$this5 = $this->b;
		$v4 = ord($this5->s[$pos1]);
		$this6 = $this->b;
		$v5 = $v4 | ord($this6->s[$pos1 + 1]) << 8;
		$this7 = $this->b;
		$v6 = $v5 | ord($this7->s[$pos1 + 2]) << 16;
		$this8 = $this->b;
		$v7 = $v6 | ord($this8->s[$pos1 + 3]) << 24;
		$tmp1 = null;
		if(($v7 & -2147483648) !== 0) {
			$tmp1 = $v7 | -2147483648;
		} else {
			$tmp1 = $v7;
		}
		return haxe_io_FPHelper::i64ToDouble($tmp, $tmp1);
	}
	public function getFloat($pos) {
		$b = new haxe_io_BytesInput($this, $pos, 4);
		return $b->readFloat();
	}
	public function setDouble($pos, $v) {
		$i = haxe_io_FPHelper::doubleToI64($v);
		{
			$v1 = $i->low;
			{
				$this1 = $this->b;
				$this1->s[$pos] = chr($v1);
			}
			{
				$this2 = $this->b;
				$this2->s[$pos + 1] = chr($v1 >> 8);
			}
			{
				$this3 = $this->b;
				$this3->s[$pos + 2] = chr($v1 >> 16);
			}
			{
				$this4 = $this->b;
				$this4->s[$pos + 3] = chr(_hx_shift_right($v1, 24));
			}
		}
		{
			$pos1 = $pos + 4;
			$v2 = $i->high;
			{
				$this5 = $this->b;
				$this5->s[$pos1] = chr($v2);
			}
			{
				$this6 = $this->b;
				$this6->s[$pos1 + 1] = chr($v2 >> 8);
			}
			{
				$this7 = $this->b;
				$this7->s[$pos1 + 2] = chr($v2 >> 16);
			}
			{
				$this8 = $this->b;
				$this8->s[$pos1 + 3] = chr(_hx_shift_right($v2, 24));
			}
		}
	}
	public function setFloat($pos, $v) {
		$v1 = haxe_io_FPHelper::floatToI32($v);
		{
			$this1 = $this->b;
			$this1->s[$pos] = chr($v1);
		}
		{
			$this2 = $this->b;
			$this2->s[$pos + 1] = chr($v1 >> 8);
		}
		{
			$this3 = $this->b;
			$this3->s[$pos + 2] = chr($v1 >> 16);
		}
		{
			$this4 = $this->b;
			$this4->s[$pos + 3] = chr(_hx_shift_right($v1, 24));
		}
	}
	public function getUInt16($pos) {
		$this1 = $this->b;
		$tmp = ord($this1->s[$pos]);
		$this2 = $this->b;
		return $tmp | ord($this2->s[$pos + 1]) << 8;
	}
	public function setUInt16($pos, $v) {
		{
			$this1 = $this->b;
			$this1->s[$pos] = chr($v);
		}
		{
			$this2 = $this->b;
			$this2->s[$pos + 1] = chr($v >> 8);
		}
	}
	public function getInt32($pos) {
		$this1 = $this->b;
		$v = ord($this1->s[$pos]);
		$this2 = $this->b;
		$v1 = $v | ord($this2->s[$pos + 1]) << 8;
		$this3 = $this->b;
		$v2 = $v1 | ord($this3->s[$pos + 2]) << 16;
		$this4 = $this->b;
		$v3 = $v2 | ord($this4->s[$pos + 3]) << 24;
		if(($v3 & -2147483648) !== 0) {
			return $v3 | -2147483648;
		} else {
			return $v3;
		}
	}
	public function getInt64($pos) {
		$pos1 = $pos + 4;
		$this1 = $this->b;
		$v = ord($this1->s[$pos1]);
		$this2 = $this->b;
		$v1 = $v | ord($this2->s[$pos1 + 1]) << 8;
		$this3 = $this->b;
		$v2 = $v1 | ord($this3->s[$pos1 + 2]) << 16;
		$this4 = $this->b;
		$v3 = $v2 | ord($this4->s[$pos1 + 3]) << 24;
		$high = null;
		if(($v3 & -2147483648) !== 0) {
			$high = $v3 | -2147483648;
		} else {
			$high = $v3;
		}
		$this5 = $this->b;
		$v4 = ord($this5->s[$pos]);
		$this6 = $this->b;
		$v5 = $v4 | ord($this6->s[$pos + 1]) << 8;
		$this7 = $this->b;
		$v6 = $v5 | ord($this7->s[$pos + 2]) << 16;
		$this8 = $this->b;
		$v7 = $v6 | ord($this8->s[$pos + 3]) << 24;
		$low = null;
		if(($v7 & -2147483648) !== 0) {
			$low = $v7 | -2147483648;
		} else {
			$low = $v7;
		}
		$x = new haxe__Int64____Int64($high, $low);
		$this9 = $x;
		return $this9;
	}
	public function setInt32($pos, $v) {
		{
			$this1 = $this->b;
			$this1->s[$pos] = chr($v);
		}
		{
			$this2 = $this->b;
			$this2->s[$pos + 1] = chr($v >> 8);
		}
		{
			$this3 = $this->b;
			$this3->s[$pos + 2] = chr($v >> 16);
		}
		{
			$this4 = $this->b;
			$this4->s[$pos + 3] = chr(_hx_shift_right($v, 24));
		}
	}
	public function setInt64($pos, $v) {
		{
			$v1 = $v->low;
			{
				$this1 = $this->b;
				$this1->s[$pos] = chr($v1);
			}
			{
				$this2 = $this->b;
				$this2->s[$pos + 1] = chr($v1 >> 8);
			}
			{
				$this3 = $this->b;
				$this3->s[$pos + 2] = chr($v1 >> 16);
			}
			{
				$this4 = $this->b;
				$this4->s[$pos + 3] = chr(_hx_shift_right($v1, 24));
			}
		}
		{
			$pos1 = $pos + 4;
			$v2 = $v->high;
			{
				$this5 = $this->b;
				$this5->s[$pos1] = chr($v2);
			}
			{
				$this6 = $this->b;
				$this6->s[$pos1 + 1] = chr($v2 >> 8);
			}
			{
				$this7 = $this->b;
				$this7->s[$pos1 + 2] = chr($v2 >> 16);
			}
			{
				$this8 = $this->b;
				$this8->s[$pos1 + 3] = chr(_hx_shift_right($v2, 24));
			}
		}
	}
	public function getString($pos, $len) {
		$tmp = null;
		$tmp1 = null;
		if($pos >= 0) {
			$tmp1 = $len < 0;
		} else {
			$tmp1 = true;
		}
		if(!$tmp1) {
			$tmp = $pos + $len > $this->length;
		} else {
			$tmp = true;
		}
		if($tmp) {
			throw new HException(haxe_io_Error::$OutsideBounds);
		}
		$this1 = $this->b;
		return substr($this1->s, $pos, $len);
	}
	public function readString($pos, $len) {
		return $this->getString($pos, $len);
	}
	public function toString() {
		return $this->b->s;
	}
	public function toHex() {
		$s = new StringBuf();
		$chars = (new _hx_array(array()));
		$str = "0123456789abcdef";
		{
			$_g1 = 0;
			$_g = strlen($str);
			while($_g1 < $_g) {
				$_g1 = $_g1 + 1;
				$i = $_g1 - 1;
				$chars->push(_hx_char_code_at($str, $i));
				unset($i);
			}
		}
		{
			$_g11 = 0;
			$_g2 = $this->length;
			while($_g11 < $_g2) {
				$_g11 = $_g11 + 1;
				$i1 = $_g11 - 1;
				$this1 = $this->b;
				$c = ord($this1->s[$i1]);
				$s1 = $s;
				$s1->b = _hx_string_or_null($s1->b) . _hx_string_or_null(chr($chars[$c >> 4]));
				$s2 = $s;
				$s2->b = _hx_string_or_null($s2->b) . _hx_string_or_null(chr($chars[$c & 15]));
				unset($this1,$s2,$s1,$i1,$c);
			}
		}
		return $s->b;
	}
	public function getData() {
		return $this->b;
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
	static function alloc($length) {
		$x = new php__BytesData_Wrapper(str_repeat(chr(0), $length));
		$this1 = $x;
		return new haxe_io_Bytes($length, $this1);
	}
	static function ofString($s) {
		$x = new php__BytesData_Wrapper($s);
		$this1 = $x;
		$x1 = $this1;
		return new haxe_io_Bytes(strlen($x1->s), $x1);
	}
	static function ofData($b) {
		return new haxe_io_Bytes(strlen($b->s), $b);
	}
	static function fastGet($b, $pos) {
		return ord($b->s[$pos]);
	}
	function __toString() { return $this->toString(); }
}
