<?php

// Generated by Haxe 3.4.2 (git build master @ 890f8c7)
class exception_StackTools {
	public function __construct(){}
	static function truncate($stack, $pos, $fromTop = null) {
		if($fromTop === null) {
			$fromTop = false;
		}
		return exception_StackTools::phpTruncate($stack, $pos, $fromTop);
	}
	static function phpTruncate($stack, $pos, $fromTop = null) {
		if($fromTop === null) {
			$fromTop = false;
		}
		$posIndex = 0;
		$from = null;
		if($fromTop) {
			$from = $stack->length - 1;
		} else {
			$from = 0;
		}
		$till = null;
		if($fromTop) {
			$till = -1;
		} else {
			$till = $stack->length;
		}
		{
			$_g_till = null;
			$_g_current = 0;
			$_g_till = 0;
			$_g_current = $from;
			$_g_till = $till;
			while($_g_current !== $_g_till) {
				$i = null;
				if($_g_current < $_g_till) {
					$_g_current = $_g_current + 1;
					$i = $_g_current - 1;
				} else {
					$_g_current = $_g_current - 1;
					$i = $_g_current + 1;
				}
				{
					$_g = $stack[$i];
					if($_g->index === 3) {
						$methodName = _hx_deref($_g)->params[1];
						$className = _hx_deref($_g)->params[0];
						$tmp = null;
						if($className === $pos->className) {
							$tmp = $methodName === $pos->methodName;
						} else {
							$tmp = false;
						}
						if($tmp) {
							$posIndex = $i;
							break;
						}
						unset($tmp,$methodName,$className);
					}
					unset($_g);
				}
				unset($i);
			}
		}
		if($fromTop) {
			return $stack->slice(0, $posIndex + 1);
		} else {
			return $stack->slice($posIndex, null);
		}
	}
	function __toString() { return 'exception.StackTools'; }
}