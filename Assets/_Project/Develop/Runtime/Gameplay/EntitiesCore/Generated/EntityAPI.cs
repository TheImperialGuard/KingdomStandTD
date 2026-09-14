namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore
{
	public partial class Entity
	{
		public Assets._Project.Develop.Runtime.Gameplay.Features.Towers.TowerType TowerTypeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Towers.TowerType>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers.TowerTypes> TowerType => TowerTypeC.Value;

		public bool TryGetTowerType(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers.TowerTypes> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Towers.TowerType component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers.TowerTypes>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTowerType()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Towers.TowerType() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers.TowerTypes>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTowerType(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers.TowerTypes> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Towers.TowerType() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Towers.TowerLevel TowerLevelC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Towers.TowerLevel>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32> TowerLevel => TowerLevelC.Value;

		public bool TryGetTowerLevel(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Towers.TowerLevel component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTowerLevel()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Towers.TowerLevel() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTowerLevel(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Towers.TowerLevel() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Towers.TowerName TowerNameC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Towers.TowerName>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.String> TowerName => TowerNameC.Value;

		public bool TryGetTowerName(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.String> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Towers.TowerName component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.String>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTowerName()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Towers.TowerName() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.String>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTowerName(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.String> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Towers.TowerName() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Towers.SubTowerParent SubTowerParentC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Towers.SubTowerParent>();

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity SubTowerParent => SubTowerParentC.Value;

		public bool TryGetSubTowerParent(out Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Towers.SubTowerParent component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSubTowerParent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Towers.SubTowerParent() { Value = new Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSubTowerParent(Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Towers.SubTowerParent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Towers.IsSubTower IsSubTowerC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Towers.IsSubTower>();

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsSubTower()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Towers.IsSubTower() ); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Towers.SubTowerCreator SubTowerCreatorC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Towers.SubTowerCreator>();

		public System.Func<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity, Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity> SubTowerCreator => SubTowerCreatorC.Value;

		public bool TryGetSubTowerCreator(out System.Func<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity, Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Towers.SubTowerCreator component);
			if(result)
				value = component.Value;
			else
				value = default(System.Func<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity, Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSubTowerCreator(System.Func<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity, Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Towers.SubTowerCreator() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Towers.IsDemo IsDemoC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Towers.IsDemo>();

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsDemo()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Towers.IsDemo() ); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Team TeamC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Team>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Teams> Team => TeamC.Value;

		public bool TryGetTeam(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Teams> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Team component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Teams>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTeam()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Team() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Teams>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTeam(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Teams> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature.Team() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature.Statuses StatusesC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature.Statuses>();

		public Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature.StatusesList Statuses => StatusesC.Value;

		public bool TryGetStatuses(out Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature.StatusesList value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature.Statuses component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature.StatusesList);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddStatuses()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature.Statuses() { Value = new Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature.StatusesList() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddStatuses(Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature.StatusesList value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature.Statuses() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature.IsStunned IsStunnedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature.IsStunned>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> IsStunned => IsStunnedC.Value;

		public bool TryGetIsStunned(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature.IsStunned component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsStunned()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature.IsStunned() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsStunned(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature.IsStunned() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature.StunDuration StunDurationC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature.StunDuration>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> StunDuration => StunDurationC.Value;

		public bool TryGetStunDuration(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature.StunDuration component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddStunDuration()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature.StunDuration() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddStunDuration(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature.StunDuration() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature.BaseStats BaseStatsC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature.BaseStats>();

		public System.Collections.Generic.Dictionary<Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature.StatTypes, System.Single> BaseStats => BaseStatsC.Value;

		public bool TryGetBaseStats(out System.Collections.Generic.Dictionary<Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature.StatTypes, System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature.BaseStats component);
			if(result)
				value = component.Value;
			else
				value = default(System.Collections.Generic.Dictionary<Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature.StatTypes, System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddBaseStats()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature.BaseStats() { Value = new System.Collections.Generic.Dictionary<Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature.StatTypes, System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddBaseStats(System.Collections.Generic.Dictionary<Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature.StatTypes, System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature.BaseStats() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature.ModifiedStats ModifiedStatsC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature.ModifiedStats>();

		public System.Collections.Generic.Dictionary<Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature.StatTypes, System.Single> ModifiedStats => ModifiedStatsC.Value;

		public bool TryGetModifiedStats(out System.Collections.Generic.Dictionary<Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature.StatTypes, System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature.ModifiedStats component);
			if(result)
				value = component.Value;
			else
				value = default(System.Collections.Generic.Dictionary<Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature.StatTypes, System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddModifiedStats()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature.ModifiedStats() { Value = new System.Collections.Generic.Dictionary<Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature.StatTypes, System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddModifiedStats(System.Collections.Generic.Dictionary<Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature.StatTypes, System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature.ModifiedStats() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature.AttacksPerSecond AttacksPerSecondC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature.AttacksPerSecond>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AttacksPerSecond => AttacksPerSecondC.Value;

		public bool TryGetAttacksPerSecond(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature.AttacksPerSecond component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttacksPerSecond()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature.AttacksPerSecond() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttacksPerSecond(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature.AttacksPerSecond() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.ShootPoint ShootPointC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.ShootPoint>();

		public UnityEngine.Transform ShootPoint => ShootPointC.Value;

		public bool TryGetShootPoint(out UnityEngine.Transform value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.ShootPoint component);
			if(result)
				value = component.Value;
			else
				value = default(UnityEngine.Transform);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddShootPoint(UnityEngine.Transform value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.ShootPoint() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.InstantShotDirection InstantShotDirectionC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.InstantShotDirection>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.InstantShotDirectionArgs> InstantShotDirection => InstantShotDirectionC.Value;

		public bool TryGetInstantShotDirection(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.InstantShotDirectionArgs> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.InstantShotDirection component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.InstantShotDirectionArgs>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInstantShotDirection()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.InstantShotDirection() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.InstantShotDirectionArgs>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInstantShotDirection(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.InstantShotDirectionArgs> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.InstantShotDirection() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.InstantShootRange InstantShootRangeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.InstantShootRange>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> InstantShootRange => InstantShootRangeC.Value;

		public bool TryGetInstantShootRange(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.InstantShootRange component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInstantShootRange()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.InstantShootRange() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInstantShootRange(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.InstantShootRange() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.ProjectileSpeed ProjectileSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.ProjectileSpeed>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> ProjectileSpeed => ProjectileSpeedC.Value;

		public bool TryGetProjectileSpeed(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.ProjectileSpeed component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddProjectileSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.ProjectileSpeed() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddProjectileSpeed(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.ProjectileSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.IsProjectile IsProjectileC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.IsProjectile>();

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsProjectile()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.IsProjectile() ); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.Owner OwnerC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.Owner>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity> Owner => OwnerC.Value;

		public bool TryGetOwner(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.Owner component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddOwner()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.Owner() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddOwner(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.Owner() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.AimingPoint AimingPointC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.AimingPoint>();

		public UnityEngine.Transform AimingPoint => AimingPointC.Value;

		public bool TryGetAimingPoint(out UnityEngine.Transform value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.AimingPoint component);
			if(result)
				value = component.Value;
			else
				value = default(UnityEngine.Transform);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAimingPoint(UnityEngine.Transform value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.AimingPoint() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.ShootingRangeZoneComponent ShootingRangeZoneC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.ShootingRangeZoneComponent>();

		public Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.ShootingRangeZone ShootingRangeZone => ShootingRangeZoneC.Value;

		public bool TryGetShootingRangeZone(out Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.ShootingRangeZone value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.ShootingRangeZoneComponent component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.ShootingRangeZone);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddShootingRangeZone(Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.ShootingRangeZone value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.ShootingRangeZoneComponent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.BallisticTrajectory BallisticTrajectoryC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.BallisticTrajectory>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.Ballistic.TrajectoryResult> BallisticTrajectory => BallisticTrajectoryC.Value;

		public bool TryGetBallisticTrajectory(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.Ballistic.TrajectoryResult> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.BallisticTrajectory component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.Ballistic.TrajectoryResult>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddBallisticTrajectory()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.BallisticTrajectory() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.Ballistic.TrajectoryResult>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddBallisticTrajectory(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.Ballistic.TrajectoryResult> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.BallisticTrajectory() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.BallisticTrajectoryMaxHeight BallisticTrajectoryMaxHeightC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.BallisticTrajectoryMaxHeight>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> BallisticTrajectoryMaxHeight => BallisticTrajectoryMaxHeightC.Value;

		public bool TryGetBallisticTrajectoryMaxHeight(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.BallisticTrajectoryMaxHeight component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddBallisticTrajectoryMaxHeight()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.BallisticTrajectoryMaxHeight() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddBallisticTrajectoryMaxHeight(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.BallisticTrajectoryMaxHeight() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.BodyCollider BodyColliderC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.BodyCollider>();

		public UnityEngine.CapsuleCollider BodyCollider => BodyColliderC.Value;

		public bool TryGetBodyCollider(out UnityEngine.CapsuleCollider value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.BodyCollider component);
			if(result)
				value = component.Value;
			else
				value = default(UnityEngine.CapsuleCollider);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddBodyCollider(UnityEngine.CapsuleCollider value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.BodyCollider() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.BoxColliderComponent BoxColliderC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.BoxColliderComponent>();

		public UnityEngine.BoxCollider BoxCollider => BoxColliderC.Value;

		public bool TryGetBoxCollider(out UnityEngine.BoxCollider value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.BoxColliderComponent component);
			if(result)
				value = component.Value;
			else
				value = default(UnityEngine.BoxCollider);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddBoxCollider(UnityEngine.BoxCollider value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.BoxColliderComponent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactsDetectingMask ContactsDetectingMaskC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactsDetectingMask>();

		public UnityEngine.LayerMask ContactsDetectingMask => ContactsDetectingMaskC.Value;

		public bool TryGetContactsDetectingMask(out UnityEngine.LayerMask value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactsDetectingMask component);
			if(result)
				value = component.Value;
			else
				value = default(UnityEngine.LayerMask);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddContactsDetectingMask(UnityEngine.LayerMask value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactsDetectingMask() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactCollidersBuffer ContactCollidersBufferC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactCollidersBuffer>();

		public Assets._Project.Develop.Runtime.Utilities.Buffer<UnityEngine.Collider> ContactCollidersBuffer => ContactCollidersBufferC.Value;

		public bool TryGetContactCollidersBuffer(out Assets._Project.Develop.Runtime.Utilities.Buffer<UnityEngine.Collider> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactCollidersBuffer component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Buffer<UnityEngine.Collider>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddContactCollidersBuffer(Assets._Project.Develop.Runtime.Utilities.Buffer<UnityEngine.Collider> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactCollidersBuffer() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactEntitiesBuffer ContactEntitiesBufferC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactEntitiesBuffer>();

		public Assets._Project.Develop.Runtime.Utilities.Buffer<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity> ContactEntitiesBuffer => ContactEntitiesBufferC.Value;

		public bool TryGetContactEntitiesBuffer(out Assets._Project.Develop.Runtime.Utilities.Buffer<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactEntitiesBuffer component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Buffer<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddContactEntitiesBuffer(Assets._Project.Develop.Runtime.Utilities.Buffer<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.ContactEntitiesBuffer() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.DeathMask DeathMaskC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.DeathMask>();

		public UnityEngine.LayerMask DeathMask => DeathMaskC.Value;

		public bool TryGetDeathMask(out UnityEngine.LayerMask value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.DeathMask component);
			if(result)
				value = component.Value;
			else
				value = default(UnityEngine.LayerMask);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDeathMask(UnityEngine.LayerMask value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.DeathMask() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchDeathMask IsTouchDeathMaskC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchDeathMask>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> IsTouchDeathMask => IsTouchDeathMaskC.Value;

		public bool TryGetIsTouchDeathMask(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchDeathMask component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsTouchDeathMask()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchDeathMask() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsTouchDeathMask(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchDeathMask() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchAnotherTeam IsTouchAnotherTeamC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchAnotherTeam>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> IsTouchAnotherTeam => IsTouchAnotherTeamC.Value;

		public bool TryGetIsTouchAnotherTeam(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchAnotherTeam component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsTouchAnotherTeam()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchAnotherTeam() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsTouchAnotherTeam(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Sensors.IsTouchAnotherTeam() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles.ProjectileType ProjectileTypeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles.ProjectileType>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles.ProjectilesTypes> ProjectileType => ProjectileTypeC.Value;

		public bool TryGetProjectileType(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles.ProjectilesTypes> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles.ProjectileType component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles.ProjectilesTypes>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddProjectileType()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles.ProjectileType() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles.ProjectilesTypes>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddProjectileType(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles.ProjectilesTypes> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles.ProjectileType() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MoveDirection MoveDirectionC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MoveDirection>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3> MoveDirection => MoveDirectionC.Value;

		public bool TryGetMoveDirection(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MoveDirection component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMoveDirection()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MoveDirection() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMoveDirection(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MoveDirection() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MoveSpeed MoveSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MoveSpeed>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> MoveSpeed => MoveSpeedC.Value;

		public bool TryGetMoveSpeed(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MoveSpeed component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMoveSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MoveSpeed() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMoveSpeed(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.MoveSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.SpeedAcceleration SpeedAccelerationC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.SpeedAcceleration>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> SpeedAcceleration => SpeedAccelerationC.Value;

		public bool TryGetSpeedAcceleration(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.SpeedAcceleration component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSpeedAcceleration()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.SpeedAcceleration() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSpeedAcceleration(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.SpeedAcceleration() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanMove CanMoveC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanMove>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition CanMove => CanMoveC.Value;

		public bool TryGetCanMove(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanMove component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanMove(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanMove() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.IsMoving IsMovingC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.IsMoving>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> IsMoving => IsMovingC.Value;

		public bool TryGetIsMoving(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.IsMoving component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsMoving()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.IsMoving() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsMoving(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.IsMoving() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.RotationDirection RotationDirectionC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.RotationDirection>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3> RotationDirection => RotationDirectionC.Value;

		public bool TryGetRotationDirection(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.RotationDirection component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddRotationDirection()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.RotationDirection() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddRotationDirection(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.RotationDirection() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.RotationSpeed RotationSpeedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.RotationSpeed>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> RotationSpeed => RotationSpeedC.Value;

		public bool TryGetRotationSpeed(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.RotationSpeed component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddRotationSpeed()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.RotationSpeed() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddRotationSpeed(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.RotationSpeed() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanRotate CanRotateC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanRotate>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition CanRotate => CanRotateC.Value;

		public bool TryGetCanRotate(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanRotate component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanRotate(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature.CanRotate() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.CurrentWaypoint CurrentWaypointC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.CurrentWaypoint>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.Waypoint> CurrentWaypoint => CurrentWaypointC.Value;

		public bool TryGetCurrentWaypoint(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.Waypoint> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.CurrentWaypoint component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.Waypoint>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCurrentWaypoint()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.CurrentWaypoint() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.Waypoint>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCurrentWaypoint(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.Waypoint> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.CurrentWaypoint() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.Waypoints WaypointsC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.Waypoints>();

		public System.Collections.Generic.List<Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.Waypoint> Waypoints => WaypointsC.Value;

		public bool TryGetWaypoints(out System.Collections.Generic.List<Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.Waypoint> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.Waypoints component);
			if(result)
				value = component.Value;
			else
				value = default(System.Collections.Generic.List<Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.Waypoint>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWaypoints()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.Waypoints() { Value = new System.Collections.Generic.List<Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.Waypoint>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWaypoints(System.Collections.Generic.List<Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.Waypoint> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.Waypoints() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.ReachedWaypoints ReachedWaypointsC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.ReachedWaypoints>();

		public System.Collections.Generic.List<Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.Waypoint> ReachedWaypoints => ReachedWaypointsC.Value;

		public bool TryGetReachedWaypoints(out System.Collections.Generic.List<Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.Waypoint> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.ReachedWaypoints component);
			if(result)
				value = component.Value;
			else
				value = default(System.Collections.Generic.List<Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.Waypoint>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddReachedWaypoints()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.ReachedWaypoints() { Value = new System.Collections.Generic.List<Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.Waypoint>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddReachedWaypoints(System.Collections.Generic.List<Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.Waypoint> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.ReachedWaypoints() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.IsPathFinished IsPathFinishedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.IsPathFinished>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> IsPathFinished => IsPathFinishedC.Value;

		public bool TryGetIsPathFinished(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.IsPathFinished component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsPathFinished()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.IsPathFinished() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsPathFinished(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.IsPathFinished() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.DamageOnFinishPath DamageOnFinishPathC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.DamageOnFinishPath>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32> DamageOnFinishPath => DamageOnFinishPathC.Value;

		public bool TryGetDamageOnFinishPath(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.DamageOnFinishPath component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDamageOnFinishPath()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.DamageOnFinishPath() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDamageOnFinishPath(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.DamageOnFinishPath() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.WaypointsOffset WaypointsOffsetC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.WaypointsOffset>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3> WaypointsOffset => WaypointsOffsetC.Value;

		public bool TryGetWaypointsOffset(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.WaypointsOffset component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWaypointsOffset()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.WaypointsOffset() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddWaypointsOffset(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<UnityEngine.Vector3> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.WaypointsOffset() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.CurrentPathDistance CurrentPathDistanceC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.CurrentPathDistance>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> CurrentPathDistance => CurrentPathDistanceC.Value;

		public bool TryGetCurrentPathDistance(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.CurrentPathDistance component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCurrentPathDistance()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.CurrentPathDistance() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCurrentPathDistance(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.CurrentPathDistance() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.TotalPathDistance TotalPathDistanceC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.TotalPathDistance>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> TotalPathDistance => TotalPathDistanceC.Value;

		public bool TryGetTotalPathDistance(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.TotalPathDistance component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTotalPathDistance()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.TotalPathDistance() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTotalPathDistance(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.TotalPathDistance() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.TraveledPathDistance TraveledPathDistanceC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.TraveledPathDistance>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> TraveledPathDistance => TraveledPathDistanceC.Value;

		public bool TryGetTraveledPathDistance(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.TraveledPathDistance component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTraveledPathDistance()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.TraveledPathDistance() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTraveledPathDistance(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation.TraveledPathDistance() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LastingDamage.LastingDamage LastingDamageC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LastingDamage.LastingDamage>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> LastingDamage => LastingDamageC.Value;

		public bool TryGetLastingDamage(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LastingDamage.LastingDamage component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddLastingDamage()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LastingDamage.LastingDamage() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddLastingDamage(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LastingDamage.LastingDamage() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LastingDamage.LastingDamageInitialTime LastingDamageInitialTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LastingDamage.LastingDamageInitialTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> LastingDamageInitialTime => LastingDamageInitialTimeC.Value;

		public bool TryGetLastingDamageInitialTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LastingDamage.LastingDamageInitialTime component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddLastingDamageInitialTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LastingDamage.LastingDamageInitialTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddLastingDamageInitialTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LastingDamage.LastingDamageInitialTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.LastingDamage.LastingDamageInterval LastingDamageIntervalC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.LastingDamage.LastingDamageInterval>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> LastingDamageInterval => LastingDamageIntervalC.Value;

		public bool TryGetLastingDamageInterval(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.LastingDamage.LastingDamageInterval component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddLastingDamageInterval()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LastingDamage.LastingDamageInterval() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddLastingDamageInterval(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.LastingDamage.LastingDamageInterval() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.IsInteractable IsInteractableC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.IsInteractable>();

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsInteractable()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.IsInteractable() ); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.CanInteract CanInteractC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.CanInteract>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition CanInteract => CanInteractC.Value;

		public bool TryGetCanInteract(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.CanInteract component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanInteract(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.CanInteract() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.InteractRequest InteractRequestC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.InteractRequest>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent InteractRequest => InteractRequestC.Value;

		public bool TryGetInteractRequest(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.InteractRequest component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInteractRequest()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.InteractRequest() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInteractRequest(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.InteractRequest() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.InteractEvent InteractEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.InteractEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent InteractEvent => InteractEventC.Value;

		public bool TryGetInteractEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.InteractEvent component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInteractEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.InteractEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInteractEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.InteractEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.InteractiveAction InteractiveActionC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.InteractiveAction>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.IInteractAction> InteractiveAction => InteractiveActionC.Value;

		public bool TryGetInteractiveAction(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.IInteractAction> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.InteractiveAction component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.IInteractAction>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInteractiveAction()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.InteractiveAction() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.IInteractAction>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInteractiveAction(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.IInteractAction> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Interactables.InteractiveAction() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.GoldEarning.GoldOnDeath GoldOnDeathC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.GoldEarning.GoldOnDeath>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32> GoldOnDeath => GoldOnDeathC.Value;

		public bool TryGetGoldOnDeath(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.GoldEarning.GoldOnDeath component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGoldOnDeath()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GoldEarning.GoldOnDeath() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddGoldOnDeath(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.GoldEarning.GoldOnDeath() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.SpawnProcessTimer SpawnProcessTimerC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.SpawnProcessTimer>();

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSpawnProcessTimer(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> initialTime,Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> currentTime)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.SpawnProcessTimer() {InitialTime = initialTime, CurrentTime = currentTime}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.InSpawnProcess InSpawnProcessC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.InSpawnProcess>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> InSpawnProcess => InSpawnProcessC.Value;

		public bool TryGetInSpawnProcess(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.InSpawnProcess component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInSpawnProcess()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.InSpawnProcess() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInSpawnProcess(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.InSpawnProcess() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.CurrentHealth CurrentHealthC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.CurrentHealth>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> CurrentHealth => CurrentHealthC.Value;

		public bool TryGetCurrentHealth(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.CurrentHealth component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCurrentHealth()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.CurrentHealth() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCurrentHealth(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.CurrentHealth() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.MaxHealth MaxHealthC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.MaxHealth>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> MaxHealth => MaxHealthC.Value;

		public bool TryGetMaxHealth(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.MaxHealth component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMaxHealth()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.MaxHealth() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMaxHealth(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.MaxHealth() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.IsDead IsDeadC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.IsDead>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> IsDead => IsDeadC.Value;

		public bool TryGetIsDead(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.IsDead component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsDead()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.IsDead() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddIsDead(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.IsDead() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.MustDie MustDieC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.MustDie>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition MustDie => MustDieC.Value;

		public bool TryGetMustDie(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.MustDie component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMustDie(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.MustDie() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.MustSelfRelease MustSelfReleaseC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.MustSelfRelease>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition MustSelfRelease => MustSelfReleaseC.Value;

		public bool TryGetMustSelfRelease(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.MustSelfRelease component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMustSelfRelease(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.MustSelfRelease() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.SelfReleaseRequested SelfReleaseRequestedC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.SelfReleaseRequested>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> SelfReleaseRequested => SelfReleaseRequestedC.Value;

		public bool TryGetSelfReleaseRequested(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.SelfReleaseRequested component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSelfReleaseRequested()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.SelfReleaseRequested() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSelfReleaseRequested(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.SelfReleaseRequested() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.DeathProcessInitialTime DeathProcessInitialTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.DeathProcessInitialTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> DeathProcessInitialTime => DeathProcessInitialTimeC.Value;

		public bool TryGetDeathProcessInitialTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.DeathProcessInitialTime component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDeathProcessInitialTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.DeathProcessInitialTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDeathProcessInitialTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.DeathProcessInitialTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.DeathProcessCurrentTime DeathProcessCurrentTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.DeathProcessCurrentTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> DeathProcessCurrentTime => DeathProcessCurrentTimeC.Value;

		public bool TryGetDeathProcessCurrentTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.DeathProcessCurrentTime component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDeathProcessCurrentTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.DeathProcessCurrentTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDeathProcessCurrentTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.DeathProcessCurrentTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.InDeathProcces InDeathProccesC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.InDeathProcces>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> InDeathProcces => InDeathProccesC.Value;

		public bool TryGetInDeathProcces(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.InDeathProcces component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInDeathProcces()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.InDeathProcces() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInDeathProcces(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.InDeathProcces() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.DisableCollidersOnDeath DisableCollidersOnDeathC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.DisableCollidersOnDeath>();

		public System.Collections.Generic.List<UnityEngine.Collider> DisableCollidersOnDeath => DisableCollidersOnDeathC.Value;

		public bool TryGetDisableCollidersOnDeath(out System.Collections.Generic.List<UnityEngine.Collider> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.DisableCollidersOnDeath component);
			if(result)
				value = component.Value;
			else
				value = default(System.Collections.Generic.List<UnityEngine.Collider>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDisableCollidersOnDeath()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.DisableCollidersOnDeath() { Value = new System.Collections.Generic.List<UnityEngine.Collider>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDisableCollidersOnDeath(System.Collections.Generic.List<UnityEngine.Collider> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle.DisableCollidersOnDeath() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Enemies.EnemyName EnemyNameC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Enemies.EnemyName>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.String> EnemyName => EnemyNameC.Value;

		public bool TryGetEnemyName(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.String> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Enemies.EnemyName component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.String>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddEnemyName()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Enemies.EnemyName() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.String>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddEnemyName(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.String> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Enemies.EnemyName() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ContactStatusInjection.BodyContactInjectingStatuses BodyContactInjectingStatusesC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ContactStatusInjection.BodyContactInjectingStatuses>();

		public System.Collections.Generic.List<Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature.StatusesTypes> BodyContactInjectingStatuses => BodyContactInjectingStatusesC.Value;

		public bool TryGetBodyContactInjectingStatuses(out System.Collections.Generic.List<Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature.StatusesTypes> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ContactStatusInjection.BodyContactInjectingStatuses component);
			if(result)
				value = component.Value;
			else
				value = default(System.Collections.Generic.List<Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature.StatusesTypes>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddBodyContactInjectingStatuses()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ContactStatusInjection.BodyContactInjectingStatuses() { Value = new System.Collections.Generic.List<Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature.StatusesTypes>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddBodyContactInjectingStatuses(System.Collections.Generic.List<Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature.StatusesTypes> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ContactStatusInjection.BodyContactInjectingStatuses() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ContactDamage.BodyContactDamage BodyContactDamageC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ContactDamage.BodyContactDamage>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> BodyContactDamage => BodyContactDamageC.Value;

		public bool TryGetBodyContactDamage(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ContactDamage.BodyContactDamage component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddBodyContactDamage()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ContactDamage.BodyContactDamage() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddBodyContactDamage(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ContactDamage.BodyContactDamage() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownInitialTime AttackCooldownInitialTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownInitialTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AttackCooldownInitialTime => AttackCooldownInitialTimeC.Value;

		public bool TryGetAttackCooldownInitialTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownInitialTime component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackCooldownInitialTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownInitialTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackCooldownInitialTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownInitialTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownModifiedTime AttackCooldownModifiedTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownModifiedTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AttackCooldownModifiedTime => AttackCooldownModifiedTimeC.Value;

		public bool TryGetAttackCooldownModifiedTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownModifiedTime component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackCooldownModifiedTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownModifiedTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackCooldownModifiedTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownModifiedTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownCurrentTime AttackCooldownCurrentTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownCurrentTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AttackCooldownCurrentTime => AttackCooldownCurrentTimeC.Value;

		public bool TryGetAttackCooldownCurrentTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownCurrentTime component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackCooldownCurrentTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownCurrentTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackCooldownCurrentTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCooldownCurrentTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackCooldown InAttackCooldownC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackCooldown>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> InAttackCooldown => InAttackCooldownC.Value;

		public bool TryGetInAttackCooldown(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackCooldown component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInAttackCooldown()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackCooldown() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInAttackCooldown(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackCooldown() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.MustCancelAttack MustCancelAttackC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.MustCancelAttack>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition MustCancelAttack => MustCancelAttackC.Value;

		public bool TryGetMustCancelAttack(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.MustCancelAttack component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMustCancelAttack(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.MustCancelAttack() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCancelEvent AttackCancelEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCancelEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent AttackCancelEvent => AttackCancelEventC.Value;

		public bool TryGetAttackCancelEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCancelEvent component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackCancelEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCancelEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackCancelEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackCancelEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InstantAttackDamage InstantAttackDamageC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InstantAttackDamage>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> InstantAttackDamage => InstantAttackDamageC.Value;

		public bool TryGetInstantAttackDamage(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InstantAttackDamage component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInstantAttackDamage()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InstantAttackDamage() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInstantAttackDamage(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InstantAttackDamage() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InstantAttackDamageType InstantAttackDamageTypeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InstantAttackDamageType>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageTypes> InstantAttackDamageType => InstantAttackDamageTypeC.Value;

		public bool TryGetInstantAttackDamageType(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageTypes> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InstantAttackDamageType component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageTypes>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInstantAttackDamageType()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InstantAttackDamageType() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageTypes>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInstantAttackDamageType(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageTypes> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InstantAttackDamageType() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackRequest StartAttackRequestC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackRequest>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent StartAttackRequest => StartAttackRequestC.Value;

		public bool TryGetStartAttackRequest(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackRequest component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddStartAttackRequest()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackRequest() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddStartAttackRequest(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackRequest() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackEvent StartAttackEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent StartAttackEvent => StartAttackEventC.Value;

		public bool TryGetStartAttackEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackEvent component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddStartAttackEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddStartAttackEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.StartAttackEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.CanStartAttack CanStartAttackC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.CanStartAttack>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition CanStartAttack => CanStartAttackC.Value;

		public bool TryGetCanStartAttack(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.CanStartAttack component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanStartAttack(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.CanStartAttack() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.EndAttackEvent EndAttackEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.EndAttackEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent EndAttackEvent => EndAttackEventC.Value;

		public bool TryGetEndAttackEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.EndAttackEvent component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddEndAttackEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.EndAttackEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddEndAttackEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.EndAttackEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessInitialTime AttackProcessInitialTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessInitialTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AttackProcessInitialTime => AttackProcessInitialTimeC.Value;

		public bool TryGetAttackProcessInitialTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessInitialTime component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackProcessInitialTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessInitialTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackProcessInitialTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessInitialTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessModifiedTime AttackProcessModifiedTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessModifiedTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AttackProcessModifiedTime => AttackProcessModifiedTimeC.Value;

		public bool TryGetAttackProcessModifiedTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessModifiedTime component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackProcessModifiedTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessModifiedTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackProcessModifiedTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessModifiedTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessCurrentTime AttackProcessCurrentTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessCurrentTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AttackProcessCurrentTime => AttackProcessCurrentTimeC.Value;

		public bool TryGetAttackProcessCurrentTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessCurrentTime component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackProcessCurrentTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessCurrentTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackProcessCurrentTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackProcessCurrentTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackProcess InAttackProcessC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackProcess>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> InAttackProcess => InAttackProcessC.Value;

		public bool TryGetInAttackProcess(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackProcess component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInAttackProcess()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackProcess() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInAttackProcess(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.InAttackProcess() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayTime AttackDelayTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AttackDelayTime => AttackDelayTimeC.Value;

		public bool TryGetAttackDelayTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayTime component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackDelayTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackDelayTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayModifiedTime AttackDelayModifiedTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayModifiedTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AttackDelayModifiedTime => AttackDelayModifiedTimeC.Value;

		public bool TryGetAttackDelayModifiedTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayModifiedTime component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackDelayModifiedTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayModifiedTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackDelayModifiedTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayModifiedTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayEndEvent AttackDelayEndEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayEndEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent AttackDelayEndEvent => AttackDelayEndEventC.Value;

		public bool TryGetAttackDelayEndEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayEndEvent component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackDelayEndEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayEndEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAttackDelayEndEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.Attack.AttackDelayEndEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageRequest TakeDamageRequestC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageRequest>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<System.Single> TakeDamageRequest => TakeDamageRequestC.Value;

		public bool TryGetTakeDamageRequest(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageRequest component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTakeDamageRequest()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageRequest() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTakeDamageRequest(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageRequest() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageEvent TakeDamageEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<System.Single> TakeDamageEvent => TakeDamageEventC.Value;

		public bool TryGetTakeDamageEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageEvent component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTakeDamageEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTakeDamageEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.TakeDamageEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.CanApplyDamage CanApplyDamageC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.CanApplyDamage>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition CanApplyDamage => CanApplyDamageC.Value;

		public bool TryGetCanApplyDamage(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.CanApplyDamage component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanApplyDamage(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.CanApplyDamage() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageResistanceType DamageResistanceTypeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageResistanceType>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageTypes> DamageResistanceType => DamageResistanceTypeC.Value;

		public bool TryGetDamageResistanceType(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageTypes> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageResistanceType component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageTypes>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDamageResistanceType()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageResistanceType() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageTypes>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDamageResistanceType(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageTypes> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageResistanceType() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageResistanceIndex DamageResistanceIndexC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageResistanceIndex>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> DamageResistanceIndex => DamageResistanceIndexC.Value;

		public bool TryGetDamageResistanceIndex(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageResistanceIndex component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDamageResistanceIndex()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageResistanceIndex() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddDamageResistanceIndex(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage.DamageResistanceIndex() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.AbilitiesComponent AbilitiesC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.AbilitiesComponent>();

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.AbilitiesList Abilities => AbilitiesC.Value;

		public bool TryGetAbilities(out Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.AbilitiesList value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.AbilitiesComponent component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.AbilitiesList);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAbilities()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.AbilitiesComponent() { Value = new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.AbilitiesList() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAbilities(Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.AbilitiesList value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.AbilitiesComponent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityCooldownTimer FirstAbilityCooldownTimerC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityCooldownTimer>();

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddFirstAbilityCooldownTimer(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> initialTime,Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> modifiedTime,Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> currentTime)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityCooldownTimer() {InitialTime = initialTime, ModifiedTime = modifiedTime, CurrentTime = currentTime}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityCooldownTimer SecondAbilityCooldownTimerC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityCooldownTimer>();

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSecondAbilityCooldownTimer(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> initialTime,Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> modifiedTime,Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> currentTime)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityCooldownTimer() {InitialTime = initialTime, ModifiedTime = modifiedTime, CurrentTime = currentTime}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.InFirstAbilityCooldown InFirstAbilityCooldownC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.InFirstAbilityCooldown>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> InFirstAbilityCooldown => InFirstAbilityCooldownC.Value;

		public bool TryGetInFirstAbilityCooldown(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.InFirstAbilityCooldown component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInFirstAbilityCooldown()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.InFirstAbilityCooldown() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInFirstAbilityCooldown(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.InFirstAbilityCooldown() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.InSecondAbilityCooldown InSecondAbilityCooldownC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.InSecondAbilityCooldown>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> InSecondAbilityCooldown => InSecondAbilityCooldownC.Value;

		public bool TryGetInSecondAbilityCooldown(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.InSecondAbilityCooldown component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInSecondAbilityCooldown()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.InSecondAbilityCooldown() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInSecondAbilityCooldown(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.InSecondAbilityCooldown() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.StartFirstAbilityRequest StartFirstAbilityRequestC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.StartFirstAbilityRequest>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent StartFirstAbilityRequest => StartFirstAbilityRequestC.Value;

		public bool TryGetStartFirstAbilityRequest(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.StartFirstAbilityRequest component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddStartFirstAbilityRequest()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.StartFirstAbilityRequest() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddStartFirstAbilityRequest(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.StartFirstAbilityRequest() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.StartFirstAbilityEvent StartFirstAbilityEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.StartFirstAbilityEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent StartFirstAbilityEvent => StartFirstAbilityEventC.Value;

		public bool TryGetStartFirstAbilityEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.StartFirstAbilityEvent component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddStartFirstAbilityEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.StartFirstAbilityEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddStartFirstAbilityEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.StartFirstAbilityEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.StartSecondAbilityRequest StartSecondAbilityRequestC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.StartSecondAbilityRequest>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent StartSecondAbilityRequest => StartSecondAbilityRequestC.Value;

		public bool TryGetStartSecondAbilityRequest(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.StartSecondAbilityRequest component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddStartSecondAbilityRequest()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.StartSecondAbilityRequest() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddStartSecondAbilityRequest(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.StartSecondAbilityRequest() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.StartSecondAbilityEvent StartSecondAbilityEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.StartSecondAbilityEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent StartSecondAbilityEvent => StartSecondAbilityEventC.Value;

		public bool TryGetStartSecondAbilityEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.StartSecondAbilityEvent component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddStartSecondAbilityEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.StartSecondAbilityEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddStartSecondAbilityEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.StartSecondAbilityEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.CanStartFirstAbility CanStartFirstAbilityC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.CanStartFirstAbility>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition CanStartFirstAbility => CanStartFirstAbilityC.Value;

		public bool TryGetCanStartFirstAbility(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.CanStartFirstAbility component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanStartFirstAbility(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.CanStartFirstAbility() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.CanStartSecondAbility CanStartSecondAbilityC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.CanStartSecondAbility>();

		public Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition CanStartSecondAbility => CanStartSecondAbilityC.Value;

		public bool TryGetCanStartSecondAbility(out Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.CanStartSecondAbility component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCanStartSecondAbility(Assets._Project.Develop.Runtime.Utilities.Conditions.ICompositeCondition value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.CanStartSecondAbility() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityProcessTimer FirstAbilityProcessTimerC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityProcessTimer>();

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddFirstAbilityProcessTimer(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> initialTime,Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> modifiedTime,Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> currentTime)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityProcessTimer() {InitialTime = initialTime, ModifiedTime = modifiedTime, CurrentTime = currentTime}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityProcessTimer SecondAbilityProcessTimerC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityProcessTimer>();

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSecondAbilityProcessTimer(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> initialTime,Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> modifiedTime,Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> currentTime)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityProcessTimer() {InitialTime = initialTime, ModifiedTime = modifiedTime, CurrentTime = currentTime}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.InFirstAbilityProcess InFirstAbilityProcessC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.InFirstAbilityProcess>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> InFirstAbilityProcess => InFirstAbilityProcessC.Value;

		public bool TryGetInFirstAbilityProcess(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.InFirstAbilityProcess component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInFirstAbilityProcess()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.InFirstAbilityProcess() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInFirstAbilityProcess(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.InFirstAbilityProcess() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.InSecondAbilityProcess InSecondAbilityProcessC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.InSecondAbilityProcess>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> InSecondAbilityProcess => InSecondAbilityProcessC.Value;

		public bool TryGetInSecondAbilityProcess(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.InSecondAbilityProcess component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInSecondAbilityProcess()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.InSecondAbilityProcess() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddInSecondAbilityProcess(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.InSecondAbilityProcess() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.EndFirstAbilityEvent EndFirstAbilityEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.EndFirstAbilityEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent EndFirstAbilityEvent => EndFirstAbilityEventC.Value;

		public bool TryGetEndFirstAbilityEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.EndFirstAbilityEvent component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddEndFirstAbilityEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.EndFirstAbilityEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddEndFirstAbilityEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.EndFirstAbilityEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.EndSecondAbilityEvent EndSecondAbilityEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.EndSecondAbilityEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent EndSecondAbilityEvent => EndSecondAbilityEventC.Value;

		public bool TryGetEndSecondAbilityEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.EndSecondAbilityEvent component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddEndSecondAbilityEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.EndSecondAbilityEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddEndSecondAbilityEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.EndSecondAbilityEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityDelayTime FirstAbilityDelayTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityDelayTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> FirstAbilityDelayTime => FirstAbilityDelayTimeC.Value;

		public bool TryGetFirstAbilityDelayTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityDelayTime component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddFirstAbilityDelayTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityDelayTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddFirstAbilityDelayTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityDelayTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityDelayTime SecondAbilityDelayTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityDelayTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> SecondAbilityDelayTime => SecondAbilityDelayTimeC.Value;

		public bool TryGetSecondAbilityDelayTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityDelayTime component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSecondAbilityDelayTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityDelayTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSecondAbilityDelayTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityDelayTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityDelayModifiedTime SecondAbilityDelayModifiedTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityDelayModifiedTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> SecondAbilityDelayModifiedTime => SecondAbilityDelayModifiedTimeC.Value;

		public bool TryGetSecondAbilityDelayModifiedTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityDelayModifiedTime component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSecondAbilityDelayModifiedTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityDelayModifiedTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSecondAbilityDelayModifiedTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityDelayModifiedTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityDelayModifiedTime FirstAbilityDelayModifiedTimeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityDelayModifiedTime>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> FirstAbilityDelayModifiedTime => FirstAbilityDelayModifiedTimeC.Value;

		public bool TryGetFirstAbilityDelayModifiedTime(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityDelayModifiedTime component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddFirstAbilityDelayModifiedTime()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityDelayModifiedTime() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddFirstAbilityDelayModifiedTime(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityDelayModifiedTime() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityDelayEndEvent SecondAbilityDelayEndEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityDelayEndEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent SecondAbilityDelayEndEvent => SecondAbilityDelayEndEventC.Value;

		public bool TryGetSecondAbilityDelayEndEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityDelayEndEvent component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSecondAbilityDelayEndEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityDelayEndEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSecondAbilityDelayEndEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityDelayEndEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityDelayEndEvent FirstAbilityDelayEndEventC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityDelayEndEvent>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent FirstAbilityDelayEndEvent => FirstAbilityDelayEndEventC.Value;

		public bool TryGetFirstAbilityDelayEndEvent(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityDelayEndEvent component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddFirstAbilityDelayEndEvent()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityDelayEndEvent() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddFirstAbilityDelayEndEvent(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveEvent value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityDelayEndEvent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityProjectileType FirstAbilityProjectileTypeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityProjectileType>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles.ProjectilesTypes> FirstAbilityProjectileType => FirstAbilityProjectileTypeC.Value;

		public bool TryGetFirstAbilityProjectileType(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles.ProjectilesTypes> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityProjectileType component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles.ProjectilesTypes>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddFirstAbilityProjectileType()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityProjectileType() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles.ProjectilesTypes>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddFirstAbilityProjectileType(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles.ProjectilesTypes> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.FirstAbilityProjectileType() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityProjectileType SecondAbilityProjectileTypeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityProjectileType>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles.ProjectilesTypes> SecondAbilityProjectileType => SecondAbilityProjectileTypeC.Value;

		public bool TryGetSecondAbilityProjectileType(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles.ProjectilesTypes> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityProjectileType component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles.ProjectilesTypes>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSecondAbilityProjectileType()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityProjectileType() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles.ProjectilesTypes>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddSecondAbilityProjectileType(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles.ProjectilesTypes> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.SecondAbilityProjectileType() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AOE.AreaEffectRadius AreaEffectRadiusC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AOE.AreaEffectRadius>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> AreaEffectRadius => AreaEffectRadiusC.Value;

		public bool TryGetAreaEffectRadius(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AOE.AreaEffectRadius component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAreaEffectRadius()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AOE.AreaEffectRadius() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddAreaEffectRadius(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AOE.AreaEffectRadius() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AOE.MustStunOnAttack MustStunOnAttackC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AOE.MustStunOnAttack>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> MustStunOnAttack => MustStunOnAttackC.Value;

		public bool TryGetMustStunOnAttack(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AOE.MustStunOnAttack component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMustStunOnAttack()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AOE.MustStunOnAttack() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMustStunOnAttack(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AOE.MustStunOnAttack() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AOE.MustCreateFragmentsOnProjectileDeath MustCreateFragmentsOnProjectileDeathC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AOE.MustCreateFragmentsOnProjectileDeath>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> MustCreateFragmentsOnProjectileDeath => MustCreateFragmentsOnProjectileDeathC.Value;

		public bool TryGetMustCreateFragmentsOnProjectileDeath(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AOE.MustCreateFragmentsOnProjectileDeath component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMustCreateFragmentsOnProjectileDeath()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AOE.MustCreateFragmentsOnProjectileDeath() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMustCreateFragmentsOnProjectileDeath(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Boolean> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AOE.MustCreateFragmentsOnProjectileDeath() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AOE.FragmentsDamage FragmentsDamageC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AOE.FragmentsDamage>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> FragmentsDamage => FragmentsDamageC.Value;

		public bool TryGetFragmentsDamage(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AOE.FragmentsDamage component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddFragmentsDamage()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AOE.FragmentsDamage() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddFragmentsDamage(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AOE.FragmentsDamage() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AOE.FragmentsCount FragmentsCountC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AOE.FragmentsCount>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> FragmentsCount => FragmentsCountC.Value;

		public bool TryGetFragmentsCount(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AOE.FragmentsCount component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddFragmentsCount()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AOE.FragmentsCount() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddFragmentsCount(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Single> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AOE.FragmentsCount() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AI.CurrentTarget CurrentTargetC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AI.CurrentTarget>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity> CurrentTarget => CurrentTargetC.Value;

		public bool TryGetCurrentTarget(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AI.CurrentTarget component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCurrentTarget()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AI.CurrentTarget() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddCurrentTarget(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AI.CurrentTarget() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AI.MaxTargets MaxTargetsC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AI.MaxTargets>();

		public Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32> MaxTargets => MaxTargetsC.Value;

		public bool TryGetMaxTargets(out Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AI.MaxTargets component);
			if(result)
				value = component.Value;
			else
				value = default(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMaxTargets()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AI.MaxTargets() { Value = new Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddMaxTargets(Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<System.Int32> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AI.MaxTargets() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.Features.AI.TargetsForExclude TargetsForExcludeC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.Features.AI.TargetsForExclude>();

		public System.Collections.Generic.List<Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity>> TargetsForExclude => TargetsForExcludeC.Value;

		public bool TryGetTargetsForExclude(out System.Collections.Generic.List<Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity>> value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.Features.AI.TargetsForExclude component);
			if(result)
				value = component.Value;
			else
				value = default(System.Collections.Generic.List<Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity>>);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTargetsForExclude()
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AI.TargetsForExclude() { Value = new System.Collections.Generic.List<Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity>>() }); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTargetsForExclude(System.Collections.Generic.List<Assets._Project.Develop.Runtime.Utilities.Reactive.ReactiveVariable<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity>> value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.Features.AI.TargetsForExclude() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Common.RigidbodyComponent RigidbodyC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Common.RigidbodyComponent>();

		public UnityEngine.Rigidbody Rigidbody => RigidbodyC.Value;

		public bool TryGetRigidbody(out UnityEngine.Rigidbody value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Common.RigidbodyComponent component);
			if(result)
				value = component.Value;
			else
				value = default(UnityEngine.Rigidbody);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddRigidbody(UnityEngine.Rigidbody value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Common.RigidbodyComponent() {Value = value}); 
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Common.TransformComponent TransformC => GetComponent<Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Common.TransformComponent>();

		public UnityEngine.Transform Transform => TransformC.Value;

		public bool TryGetTransform(out UnityEngine.Transform value)
		{
			bool result = TryGetComponent(out Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Common.TransformComponent component);
			if(result)
				value = component.Value;
			else
				value = default(UnityEngine.Transform);
			return result;
		}

		public Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Entity AddTransform(UnityEngine.Transform value)
		{
			return AddComponent(new Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Common.TransformComponent() {Value = value}); 
		}

	}
}
