<?php

// Generated by Haxe 3.4.2 (git build master @ 890f8c7)
class cornerstone_benchmark_Benchmark implements cornerstone_integrator_interfaces_IDisposable{
	public function __construct() {}
	public function Logic_Test1() {
		return Math::$PI * Math::$PI;
	}
	public function Logic_Test2($Par) {
		return _hx_string_or_null($Par) . " Welcome";
	}
	public function Logic_Test3() {
		$day = 86400000;
		$d = Date::now();
		$d2 = Date::fromTime($d->getTime() + $day);
		return $d2;
	}
	public function Logic_Benchmark1() {
		$DateStart = Date::now();
		$x = 0.5;
		$y = 0.5;
		$t = 0.49999975;
		$t2 = 0.5;
		{
			$_g = 0;
			while($_g < 100) {
				$_g = $_g + 1;
				$i = $_g - 1;
				{
					$_g1 = 0;
					while($_g1 < 1000) {
						$_g1 = $_g1 + 1;
						$j = $_g1 - 1;
						$x1 = $t2 * Math::sin($x);
						$x2 = $x1 * Math::cos($x);
						$x3 = Math::cos($x + $y);
						$x = $t * Math::atan($x2 / ($x3 + Math::cos($x - $y) - 1.0));
						$y1 = $t2 * Math::sin($y);
						$y2 = $y1 * Math::cos($y);
						$y3 = Math::cos($x + $y);
						$y = $t * Math::atan($y2 / ($y3 + Math::cos($x - $y) - 1.0));
						unset($y3,$y2,$y1,$x3,$x2,$x1,$j);
					}
					unset($_g1);
				}
				unset($i);
			}
		}
		$tmp = Date::now()->getTime();
		return $tmp - $DateStart->getTime();
	}
	public function Logic_Benchmark2() {
		$Letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
		$d = new cornerstone_benchmark_Probs();
		$Buffer = "";
		{
			$_g = 0;
			while($_g < 100) {
				$_g = $_g + 1;
				$r = $_g - 1;
				{
					$_g1 = 0;
					while($_g1 < 1000) {
						$_g1 = $_g1 + 1;
						$i = $_g1 - 1;
						$pos = $d->getNext();
						$pos1 = Std::int($pos * strlen($Letters));
						$Buffer = _hx_string_or_null($Buffer) . _hx_string_or_null(_hx_char_at($Letters, $pos1));
						unset($pos1,$pos,$i);
					}
					unset($_g1);
				}
				$reduce = $d->getNext();
				$reduce1 = Std::int($reduce * (strlen($Buffer) / 2));
				$Buffer = _hx_substr($Buffer, strlen($Buffer) - $reduce1, $reduce1);
				unset($reduce1,$reduce,$r);
			}
		}
		return $Buffer;
	}
	public function Logic_Benchmark3() {
		$day = 86400000;
		$d = Date::now();
		$p = new cornerstone_benchmark_Probs();
		{
			$_g = 0;
			while($_g < 100) {
				$_g = $_g + 1;
				$r = $_g - 1;
				{
					$_g1 = 0;
					while($_g1 < 1000) {
						$_g1 = $_g1 + 1;
						$i = $_g1 - 1;
						$diff1 = ($p->getNext() - 0.55) * 90.0 * $day;
						$d = Date::fromTime($d->getTime() + $diff1);
						unset($i,$diff1);
					}
					unset($_g1);
				}
				unset($r);
			}
		}
		return $d;
	}
	public function Dispose() {}
	static $__rtti = "<class path=\"cornerstone.benchmark.Benchmark\" params=\"\">\x0A\x09<implements path=\"cornerstone.integrator.interfaces.IDisposable\"/>\x0A\x09<Logic_Test1 public=\"1\" set=\"method\" line=\"18\"><f a=\"\"><x path=\"Float\"/></f></Logic_Test1>\x0A\x09<Logic_Test2 public=\"1\" set=\"method\" line=\"22\"><f a=\"Par\">\x0A\x09<c path=\"String\"/>\x0A\x09<c path=\"String\"/>\x0A</f></Logic_Test2>\x0A\x09<Logic_Test3 public=\"1\" set=\"method\" line=\"26\"><f a=\"\"><c path=\"Date\"/></f></Logic_Test3>\x0A\x09<Logic_Benchmark1 public=\"1\" set=\"method\" line=\"33\"><f a=\"\"><x path=\"Float\"/></f></Logic_Benchmark1>\x0A\x09<Logic_Benchmark2 public=\"1\" set=\"method\" line=\"58\"><f a=\"\"><c path=\"String\"/></f></Logic_Benchmark2>\x0A\x09<Logic_Benchmark3 public=\"1\" set=\"method\" line=\"75\"><f a=\"\"><c path=\"Date\"/></f></Logic_Benchmark3>\x0A\x09<Dispose public=\"1\" set=\"method\" line=\"92\"><f a=\"\"><x path=\"Void\"/></f></Dispose>\x0A\x09<new public=\"1\" set=\"method\" line=\"15\"><f a=\"\"><x path=\"Void\"/></f></new>\x0A\x09<meta>\x0A\x09\x09<m n=\":directlyUsed\"/>\x0A\x09\x09<m n=\":keepSub\"/>\x0A\x09\x09<m n=\":final\"/>\x0A\x09\x09<m n=\":rtti\"/>\x0A\x09</meta>\x0A</class>";
	function __toString() { return 'cornerstone.benchmark.Benchmark'; }
}
