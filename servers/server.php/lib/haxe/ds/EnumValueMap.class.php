<?php

// Generated by Haxe 3.4.2 (git build master @ 890f8c7)
class haxe_ds_EnumValueMap extends haxe_ds_BalancedTree implements haxe_IMap{
	public function __construct() { if(!php_Boot::$skip_constructor) {
		parent::__construct();
	}}
	public function compare($k1, $k2) {
		$d = $k1->index - $k2->index;
		if($d !== 0) {
			return $d;
		}
		$p1 = Type::enumParameters($k1);
		$p2 = Type::enumParameters($k2);
		$tmp = null;
		if($p1->length === 0) {
			$tmp = $p2->length === 0;
		} else {
			$tmp = false;
		}
		if($tmp) {
			return 0;
		}
		return $this->compareArgs($p1, $p2);
	}
	public function compareArgs($a1, $a2) {
		$ld = $a1->length - $a2->length;
		if($ld !== 0) {
			return $ld;
		}
		{
			$_g1 = 0;
			$_g = $a1->length;
			while($_g1 < $_g) {
				$_g1 = $_g1 + 1;
				$i = $_g1 - 1;
				$d = $this->compareArg($a1[$i], $a2[$i]);
				if($d !== 0) {
					return $d;
				}
				unset($i,$d);
			}
		}
		return 0;
	}
	public function compareArg($v1, $v2) {
		$tmp = null;
		if(Reflect::isEnumValue($v1)) {
			$tmp = Reflect::isEnumValue($v2);
		} else {
			$tmp = false;
		}
		if($tmp) {
			return $this->compare($v1, $v2);
		} else {
			$tmp1 = null;
			if(Std::is($v1, _hx_qtype("Array"))) {
				$tmp1 = Std::is($v2, _hx_qtype("Array"));
			} else {
				$tmp1 = false;
			}
			if($tmp1) {
				return $this->compareArgs($v1, $v2);
			} else {
				return Reflect::compare($v1, $v2);
			}
		}
	}
	function __toString() { return 'haxe.ds.EnumValueMap'; }
}
