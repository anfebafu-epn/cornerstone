<?php

// Generated by Haxe 3.4.2 (git build master @ 890f8c7)
class cornerstone_integrator_helpers_Int64_Helper {
	public function __construct(){}
	static $MAX_32_PRECISION = 4294967296.0;
	static function fromFloat($f) {
		$h = Std::int($f / 4294967296.0);
		$l = Std::int($f);
		$x = new haxe__Int64____Int64($h, $l);
		$this1 = $x;
		return $this1;
	}
	static function toFloat($i) {
		$f = $i->low;
		if($f < 0) {
			$f = $f + 4294967296.0;
		}
		return $i->high * 4294967296.0 + $f;
	}
	function __toString() { return 'cornerstone.integrator.helpers.Int64_Helper'; }
}
