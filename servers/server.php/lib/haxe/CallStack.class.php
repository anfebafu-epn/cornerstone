<?php

// Generated by Haxe 3.4.2 (git build master @ 890f8c7)
class haxe_CallStack {
	public function __construct(){}
	static function callStack() {
		return haxe_CallStack::makeStack("%s");
	}
	static function exceptionStack() {
		return haxe_CallStack::makeStack("%e");
	}
	static function toString($stack) {
		$b = new StringBuf();
		{
			$_g = 0;
			while($_g < $stack->length) {
				$s = $stack[$_g];
				$_g = $_g + 1;
				$b->add("\x0ACalled from ");
				haxe_CallStack::itemToString($b, $s);
				unset($s);
			}
		}
		return $b->b;
	}
	static function itemToString($b, $s) {
		switch($s->index) {
		case 0:{
			$b->add("a C function");
		}break;
		case 1:{
			$m = _hx_deref($s)->params[0];
			{
				$b->add("module ");
				$b->add($m);
			}
		}break;
		case 2:{
			$line = _hx_deref($s)->params[2];
			$file = _hx_deref($s)->params[1];
			$s1 = _hx_deref($s)->params[0];
			{
				if($s1 !== null) {
					haxe_CallStack::itemToString($b, $s1);
					$b->add(" (");
				}
				$b->add($file);
				$b->add(" line ");
				$b->add($line);
				if($s1 !== null) {
					$b->add(")");
				}
			}
		}break;
		case 3:{
			$meth = _hx_deref($s)->params[1];
			$cname = _hx_deref($s)->params[0];
			{
				$b->add($cname);
				$b->add(".");
				$b->add($meth);
			}
		}break;
		case 4:{
			$n = _hx_deref($s)->params[0];
			{
				$b->add("local function #");
				$b->add($n);
			}
		}break;
		}
	}
	static function makeStack($s) {
		if(!isset($GLOBALS[$s])) {
			return (new _hx_array(array()));
		}
		$a = $GLOBALS[$s];
		$m = (new _hx_array(array()));
		{
			$_g1 = 0;
			$_g = null;
			if($s === "%s") {
				$_g = 2;
			} else {
				$_g = 0;
			}
			$_g2 = $a->length - $_g;
			while($_g1 < $_g2) {
				$_g1 = $_g1 + 1;
				$i = $_g1 - 1;
				$d = _hx_explode("::", $a[$i]);
				$m->unshift(haxe_StackItem::Method($d[0], $d[1]));
				unset($i,$d);
			}
		}
		return $m;
	}
	function __toString() { return 'haxe.CallStack'; }
}
